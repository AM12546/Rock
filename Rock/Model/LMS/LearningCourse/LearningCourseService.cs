﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Rock.Data;
using Rock.Enums.Lms;
using Rock.Utility;
using Rock.ViewModels.Blocks.Lms.LearningCourseRequirement;

using WebGrease.Css.Extensions;

namespace Rock.Model
{
    public partial class LearningCourseService
    {
        /// <summary>
        /// Get a list of required courses for the specified course. 
        /// </summary>
        /// <param name="courseId">The identifier of the <see cref="LearningCourse"/> to get requirements for.</param>
        /// <returns>A list of <see cref="LearningCourseRequirementBag"/> records.</returns>
        public LearningCourse GetCourseWithRequirements( int courseId )
        {
            var course = Queryable()
                .Include( a => a.LearningProgram )
                .Include( a => a.LearningClasses )
                .Include( a => a.LearningCourseRequirements.Select( cr => cr.RequiredLearningCourse ) )
                .FirstOrDefault( a => a.Id == courseId );

            return course;
        }

        /// <summary>
        /// Gets the details for a public course.
        /// </summary>
        /// <param name="courseId">The identifier of the course to get.</param>
        /// <param name="personId">The identifier of the <see cref="Person"/> to include completion status for.</param>
        /// <param name="publicOnly"><c>true</c> to include <see cref="LearningClass"/> records whose IsPublic property is true; <c>false</c> to include regardless of IsPublic.</param>
        /// <param name="semesterStartFrom">Optional filter for the next session Semester Start. Only Start Dates greater than this date will be included.</param>
        /// <param name="semesterStartTo">Optional filter for the next session Semester Start. Only Start Dates less than this date will be included.</param>
        /// <returns>A <see cref="PublicLearningCourseDetailBag"/> containing the data necessary for rendering the details.</returns>
        public PublicLearningCourseDetailBag GetPublicCourseDetails( int courseId, int? personId, bool publicOnly = true, DateTime? semesterStartFrom = null, DateTime? semesterStartTo = null )
        {
            var rockContext = ( RockContext ) Context;
            var participantService = new LearningParticipantService( rockContext );

            var mostRecentParticipation =
                !personId.HasValue ?
                default :
                participantService
                .GetClasses( personId.Value, true )
                .AsNoTracking()
                .OrderByDescending( p => p.CreatedDateTime )
                .FirstOrDefault( p => p.LearningClass.LearningCourseId == courseId );

            var now = RockDateTime.Now;
            var course = Queryable()
                .AsNoTracking()
                .Include( c => c.ImageBinaryFile )
                .Include( c => c.LearningProgram )
                .Include( c => c.Category )
                .Include( c => c.LearningCourseRequirements )
                .Include( c => c.LearningClasses )
                .Where( c => c.IsActive && c.Id == courseId )
                .Where( c => !publicOnly || c.IsPublic )
                .Select( c => new PublicLearningCourseDetailBag
                {
                    Entity = c,
                    Category = c.Category.Name,
                    CategoryColor = c.Category.HighlightColor,
                    CourseRequirements = c.LearningCourseRequirements.ToList(),
                    ImageFileGuid = c.ImageBinaryFile.Guid,
                    Program = c.LearningProgram,

                    // Limit the learning classes to only
                    // active and (optionally) only public.
                    NextSemester = c.LearningClasses
                        .Where( c2 => c2.IsActive && ( !publicOnly || c2.IsPublic ) )
                        .Select( cl => cl.LearningSemester )
                        .FirstOrDefault( s =>
                            ( s.EnrollmentCloseDate == null || s.EnrollmentCloseDate >= now ) &&
                            ( s.StartDate == null || s.StartDate >= now ) &&
                            ( !semesterStartFrom.HasValue || s.StartDate >= semesterStartFrom.Value ) &&
                            ( !semesterStartTo.HasValue || s.StartDate <= semesterStartTo.Value ) &&
                            s.LearningClasses.Any( sc => sc.LearningCourseId == c.Id )
                        )
                } )
                .FirstOrDefault();

            if ( course == null )
            {
                // This can happen if a course is still being configured.
                return new PublicLearningCourseDetailBag
                {
                    CourseRequirements = new List<LearningCourseRequirement>(),
                    Entity = new LearningCourse(),
                    Facilitators = new List<LearningParticipant>(),
                    MostRecentParticipation = new LearningParticipant(),
                    NextSemester = new LearningSemester(),
                    UnmetPrerequisites = new List<LearningCourseRequirement>()
                };
            }

            var nextSemesterQuery = new LearningClassService( rockContext )
                .Queryable()
                .Where( c => c.LearningCourseId == course.Entity.Id )
                .Select( c => c.LearningSemester );

            if ( semesterStartFrom.HasValue )
            {
                nextSemesterQuery = nextSemesterQuery
                    .Where( s => s.StartDate >= semesterStartFrom.Value );
            }

            if ( semesterStartTo.HasValue )
            {
                nextSemesterQuery = nextSemesterQuery
                    .Where( s => s.StartDate <= semesterStartTo.Value );
            }

            // Get the earliest semester with open enrollment and a future start date for this course.
            course.NextSemester = nextSemesterQuery.OrderBy( c => c.StartDate ).FirstOrDefault();

            // Remove any related classes that don't belong to this course.
            course.NextSemester.LearningClasses = course.NextSemester.LearningClasses.Where( c => c.LearningCourseId == courseId ).ToList();

            course.UnmetPrerequisites = GetUnmetCourseRequirements( personId, course.CourseRequirements );

            if ( mostRecentParticipation != null )
            {
                course.LearningCompletionStatus = mostRecentParticipation.LearningCompletionStatus;
                course.MostRecentParticipation = mostRecentParticipation;
            }

            var nextSemesterId = course.NextSemester?.Id ?? 0;

            course.Facilitators = nextSemesterId == 0 ? new List<LearningParticipant>() : participantService.GetFacilitators( courseId, nextSemesterId ).ToList();

            return course;
        }

        /// <summary>
        /// Gets a list of active, public courses for the specified program.
        /// </summary>
        /// <param name="programId">The identifier of the <see cref="LearningProgram"/> for which to return courses.</param>
        /// <param name="personId">The identifier of the <see cref="Person"/> to include completion status for.</param>
        /// <param name="publicOnly"><c>true</c> to include <see cref="LearningCourse"/> and <see cref="LearningClass"/> records whose IsPublic property is true; <c>false</c> to include regardless of IsPublic.</param>
        /// <param name="semesterStartFrom">Optional filter for the next session Semester Start. Only Start Dates greater than this date will be included.</param>
        /// <param name="semesterStartTo">Optional filter for the next session Semester Start. Only Start Dates less than this date will be included.</param>
        /// <returns>An enumerable of PublicLearningCourseBag.</returns>
        public List<PublicLearningCourseBag> GetPublicCourses( int programId, int? personId, bool publicOnly = true, DateTime? semesterStartFrom = null, DateTime? semesterStartTo = null )
        {
            var rockContext = ( RockContext ) Context;
            var orderedPersonCompletions =
                !personId.HasValue ?
                new List<LearningParticipant>().AsQueryable().OrderBy( "LearningCompletionStatus" ) :
                new LearningParticipantService( rockContext )
                .GetClasses( personId.Value )
                .AsNoTracking()
                // If the student has taken the class multiple times take in this order:
                // 'Pass' - 'Incomplete' - 'Fail'.
                .OrderBy( p =>
                            LearningCompletionStatus.Pass == p.LearningCompletionStatus ? 0 :
                            LearningCompletionStatus.Incomplete == p.LearningCompletionStatus ? 1 :
                            2 // Fail
                        );

            //  Get all Semesters for the program.
            //  Include the classes for joining to the Course.
            var semesters = new LearningSemesterService( rockContext )
                .Queryable()
                .Include( s => s.LearningClasses )
                .Where( s => s.LearningProgramId == programId );

            var unmetPrerequisiteTypes = new List<RequirementType> { RequirementType.Prerequisite, RequirementType.Equivalent };
            var now = RockDateTime.Now;
            var courses = Queryable()
                .Include( c => c.ImageBinaryFile )
                .Include( c => c.LearningProgram )
                .Include( c => c.Category )
                .Include( c => c.LearningCourseRequirements )
                .Where( c =>
                    c.IsActive
                    && c.LearningProgramId == programId
                    && ( c.IsPublic || !publicOnly ) )
                .Select( c => new PublicLearningCourseBag
                {
                    Entity = c,
                    Category = c.Category.Name,
                    CategoryColor = c.Category.HighlightColor,
                    ImageFileGuid = c.ImageBinaryFile.Guid,

                    // Get the person's completion status for this course.
                    LearningCompletionStatus = !personId.HasValue ?
                        LearningCompletionStatus.Incomplete :
                        orderedPersonCompletions
                        .FirstOrDefault( p => p.LearningClass.LearningCourseId == c.Id )
                        .LearningCompletionStatus,

                    // Get the earliest semester with open enrollment and a start date within the specified dates for this course.
                    NextSemester = semesters.FirstOrDefault( s =>
                        ( s.EnrollmentCloseDate == null || s.EnrollmentCloseDate >= now ) &&
                        s.StartDate >= now &&
                        ( !semesterStartFrom.HasValue || s.StartDate >= semesterStartFrom.Value ) &&
                        ( !semesterStartTo.HasValue || s.StartDate <= semesterStartTo.Value ) &&
                        s.LearningClasses.Any( sc => sc.LearningCourseId == c.Id && sc.IsActive && ( !publicOnly || sc.IsPublic ) )
                        ),

                    // Only Prerequisites/Equivalents where the course completions for the student aren't 'Passed'.
                    UnmetPrerequisites = c.LearningCourseRequirements
                        .Where( r =>
                            unmetPrerequisiteTypes.Contains( r.RequirementType ) &&
                            (
                                !personId.HasValue
                                || !orderedPersonCompletions.Any( comp =>
                                comp.LearningCompletionStatus == LearningCompletionStatus.Pass &&
                                comp.LearningClass.LearningCourseId == r.RequiredLearningCourseId ) ) )
                        .ToList()
                } )
                .ToList()
                // Sort in memory (after calling ToList).
                .OrderBy( c => c.Entity.Order )
                .ThenBy( c => c.Entity.Id );

            if ( courses.Any( c => c.Entity.LearningCourseRequirements.Any() ) )
            {
                // Get the required courses in a single query.
                var requiredCourseIds = courses.SelectMany( c => c.Entity.LearningCourseRequirements.Select( r => r.RequiredLearningCourseId ) );
                var requiredCourses = Queryable().Where( c => requiredCourseIds.Contains( c.Id ) );

                // Then match them up based on the course id of the required course.
                foreach ( var course in courses )
                {
                    if ( !course.Entity.LearningCourseRequirements.Any() )
                    {
                        continue;
                    }

                    course.Entity.LearningCourseRequirements.ForEach( cr =>
                        cr.RequiredLearningCourse =
                            requiredCourses.FirstOrDefault( r => r.Id == cr.RequiredLearningCourseId ) );
                }
            }

            return courses.ToList();
        }

        /// <summary>
        /// Gets a list of LearningCourseRequirements where the <see cref="Person"/> specified by the <paramref name="personId"/>
        /// hasn't completed the course.
        /// </summary>
        /// <param name="personId">The identifier of the <see cref="Person"/> to check requirement completions for.</param>
        /// <param name="courseRequirements">The List of <see cref="LearningCourseRequirement"/> records to check completions for.</param>
        /// <returns>A List of <see cref="LearningCourseRequirement"/> records that haven't been completed by the <see cref="Person"/>.</returns>
        public List<LearningCourseRequirement> GetUnmetCourseRequirements( int? personId, IEnumerable<LearningCourseRequirement> courseRequirements )
        {
            if ( courseRequirements.Any() )
            {
                var hasMissingCourseDetails = courseRequirements.Any( cr => cr.RequiredLearningCourse == null || cr.RequiredLearningCourse.Id == 0 );
                if ( hasMissingCourseDetails )
                {
                    // If there were provided LearningCourseRequirements that aren't populated with their
                    // related RequiredLearningCourse then go get that data.
                    var requiredCourseIds = courseRequirements.Select( r => r.RequiredLearningCourseId );
                    var requiredCourses = Queryable().Where( c => requiredCourseIds.Contains( c.Id ) );

                    courseRequirements.ForEach( cr =>
                    cr.RequiredLearningCourse =
                        requiredCourses.FirstOrDefault( r => r.Id == cr.RequiredLearningCourseId ) );
                }

                var completedClasses =
                    !personId.HasValue ?
                    default
                    : new LearningParticipantService( ( RockContext ) Context )
                    .GetClasses( personId.Value )
                    .AsNoTracking();

                // Any Equivalent or PreRequisite classes that aren't already passed.
                var unmetPrerequisiteTypes = new List<RequirementType> { RequirementType.Prerequisite, RequirementType.Equivalent };
                return courseRequirements.Where( cr =>
                    unmetPrerequisiteTypes.Contains( cr.RequirementType ) &&
                    !completedClasses.Any( c => c.LearningClass.LearningCourseId == cr.RequiredLearningCourseId && c.LearningCompletionStatus == LearningCompletionStatus.Pass )
                    ).ToList();
            }

            return new List<LearningCourseRequirement>();
        }

        #region Nested Classes

        /// <summary>
        /// Represents the Lava enabled data sent to the public courses list block.
        /// </summary>
        public class PublicLearningCourseBag : RockDynamic
        {
            /// <summary>
            /// Gets or sets the Learning Program entity for this bag.
            /// </summary>
            public LearningCourse Entity { get; set; }

            /// <summary>
            /// Gets or sets the category.
            /// </summary>
            public string Category { get; set; }

            /// <summary>
            /// Gets or sets the highlight color of the category.
            /// </summary>
            public string CategoryColor { get; set; }

            /// <summary>
            /// Gets or sets the completion status of the course for the current person.
            /// </summary>
            public LearningCompletionStatus? LearningCompletionStatus { get; set; }

            /// <summary>
            /// Gets or sets the link to the course details.
            /// </summary>
            public string CourseDetailsLink { get; set; }

            /// <summary>
            /// Gets or sets the link to enroll in the course.
            /// </summary>
            public string CourseEnrollmentLink { get; set; }

            /// <summary>
            /// Gets or sets the Guid for the Image file of this Program.
            /// </summary>
            public Guid? ImageFileGuid { get; set; }

            /// <summary>
            /// Gets or sets the next semester where a class is available for the course
            /// and the enrollment close date is in the future or null.
            /// </summary>
            public LearningSemester NextSemester { get; set; }

            /// <summary>
            /// Gets or sets the link for the prerequisite course if any.
            /// </summary>
            public string PrerequisiteEnrollmentLink { get; set; }

            /// <summary>
            /// Gets or sets a list of LearningCourseRequirements where the Person hasn't yet Passed the Prerequisite.
            /// </summary>
            public List<LearningCourseRequirement> UnmetPrerequisites { get; set; }
        }

        /// <summary>
        /// Represents the Lava enabled data sent to the public course detail block.
        /// </summary>
        public class PublicLearningCourseDetailBag : PublicLearningCourseBag
        {
            /// <summary>
            /// Gets or sets the link to access the class workspace for the course.
            /// </summary>
            public string ClassWorkspaceLink { get; set; }

            /// <summary>
            /// Gets or sets the list of course requirements for the requested course.
            /// </summary>
            public List<LearningCourseRequirement> CourseRequirements { get; set; }

            /// <summary>
            /// Gets or sets the sescription as an html string.
            /// </summary>
            public string DescriptionAsHtml { get; set; }

            /// <summary>
            /// Gets or sets the list of Facilitators for the course.
            /// </summary>
            public List<LearningParticipant> Facilitators { get; set; }

            /// <summary>
            /// Gets or sets the most recently attended class for the student.
            /// </summary>
            public LearningParticipant MostRecentParticipation { get; set; }

            /// <summary>
            /// Gets or sets the related <see cref="LearningProgram"/> for the course.
            /// </summary>
            public LearningProgram Program { get; set; }
        }

        #endregion
    }
}