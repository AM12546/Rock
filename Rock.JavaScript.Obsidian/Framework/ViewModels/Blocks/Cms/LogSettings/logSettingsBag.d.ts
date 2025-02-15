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

import { PublicAttributeBag } from "@Obsidian/ViewModels/Utility/publicAttributeBag";

/** The item details for the Rock Settings block. */
export type LogSettingsBag = {
    /**
     * Gets or sets the advanced settings as a JSON object that conforms
     * to the standard Microsoft logging syntax. The root object should be
     * the "Logging" node - meaning "LogLevel" should be one of the keys
     * defined at the root of this object.
     */
    advancedSettings?: string | null;

    /** Gets or sets the attributes. */
    attributes?: Record<string, PublicAttributeBag> | null;

    /** Gets or sets the attribute values. */
    attributeValues?: Record<string, string> | null;

    /**
     * Gets or sets the standard categories to log. These will be logged
     * at the level specified by Rock.ViewModels.Blocks.Cms.LogSettings.LogSettingsBag.StandardLogLevel.
     */
    categories?: string[] | null;

    /** Gets or sets the custom categories. */
    customCategories?: string[] | null;

    /** Gets or sets the identifier key of this entity. */
    idKey?: string | null;

    /**
     * Gets or sets a value indicating whether Rock will write logs to the
     * local file system.
     */
    isLocalLoggingEnabled: boolean;

    /**
     * Gets or sets a value indicating whether Rock will write logs to the
     * Observability framework.
     */
    isObservabilityLoggingEnabled: boolean;

    /** Gets or sets the maximum size of the file. */
    maxFileSize?: string | null;

    /** Gets or sets the number of log files. */
    numberOfLogFiles?: string | null;

    /** Gets or sets the selected categories. */
    selectedCategories?: string[] | null;

    /**
     * Gets or sets the log level to use for all categories that are
     * enabled for standard logging.
     */
    standardLogLevel?: string | null;
};
