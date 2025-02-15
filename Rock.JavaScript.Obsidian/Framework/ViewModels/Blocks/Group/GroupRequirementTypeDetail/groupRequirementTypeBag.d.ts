//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
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

import { ListItemBag } from "@Obsidian/ViewModels/Utility/listItemBag";
import { PublicAttributeBag } from "@Obsidian/ViewModels/Utility/publicAttributeBag";

export type GroupRequirementTypeBag = {
    /** Gets or sets the attributes. */
    attributes?: Record<string, PublicAttributeBag> | null;

    /** Gets or sets the attribute values. */
    attributeValues?: Record<string, string> | null;

    /** Gets or sets a value indicating whether the current user can administrate. */
    canAdministrate: boolean;

    /** Gets or sets a value indicating whether this requirement can expire. */
    canExpire: boolean;

    /** Gets or sets the category. */
    category?: ListItemBag | null;

    /** Gets or sets the checkbox label. This is the text that is used for the checkbox if this is a manually set requirement */
    checkboxLabel?: string | null;

    /** Gets or sets the Rock.Model.DataView. */
    dataView?: ListItemBag | null;

    /** Gets or sets the description. */
    description?: string | null;

    /** Gets or sets the text for the "Does Not Meet" workflow link. */
    doesNotMeetWorkflowLinkText?: string | null;

    /** Gets or sets "Does Not Meet" workflow type. */
    doesNotMeetWorkflowType?: ListItemBag | null;

    /** Gets or sets the number of days before the requirement is due. */
    dueDateOffsetInDays?: string | null;

    /** Gets or sets the type of due date. */
    dueDateType?: string | null;

    /** Gets or sets the number of days after the requirement is met before it expires (If CanExpire is true). NULL means never expires */
    expireInDays?: string | null;

    /** Gets or sets the icon CSS class. */
    iconCssClass?: string | null;

    /** Gets or sets the identifier key of this entity. */
    idKey?: string | null;

    /** Gets or sets the name. */
    name?: string | null;

    /** Gets or sets the negative label. This is the text that is displayed when the requirement is not met. */
    negativeLabel?: string | null;

    /** Gets or sets the positive label. This is the text that is displayed when the requirement is met. */
    positiveLabel?: string | null;

    /** Gets or sets the type of the requirement check. */
    requirementCheckType?: string | null;

    /** Gets or sets a value indicating whether this requirement type's "Does Not Meet" workflow should auto-initiate. */
    shouldAutoInitiateDoesNotMeetWorkflow: boolean;

    /** Gets or sets a value indicating whether this requirement type's "Warning" workflow should auto-initiate. */
    shouldAutoInitiateWarningWorkflow: boolean;

    /** Gets or sets the SQL expression. */
    sqlExpression?: string | null;

    /** Gets or sets the SQL help text. */
    sqlHelpHTML?: string | null;

    /** Gets or sets the summary. */
    summary?: string | null;

    /** Gets or sets the warning Rock.Model.DataView. */
    warningDataView?: ListItemBag | null;

    /** Gets or sets the warning label. */
    warningLabel?: string | null;

    /** Gets or sets the warning SQL expression. */
    warningSqlExpression?: string | null;

    /** Gets or sets the text for the "Warning" workflow link. */
    warningWorkflowLinkText?: string | null;

    /** Gets or sets "Warning" workflow type. */
    warningWorkflowType?: ListItemBag | null;
};
