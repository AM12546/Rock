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
using Rock.Utility;

namespace Rock.Communication
{
    /// <summary>
    /// Interface for communications/templates
    /// </summary>
    public interface ICommunicationDetails
    {
        #region Email Fields

        /// <summary>
        /// Gets or sets the name of the Communication
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> that represents the name of the communication.
        /// </value>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets from name.
        /// </summary>
        /// <value>
        /// From name.
        /// </value>
        string FromName { get; set; }

        /// <summary>
        /// Gets or sets from email.
        /// </summary>
        /// <value>
        /// From email.
        /// </value>
        string FromEmail { get; set; }

        /// <summary>
        /// Gets or sets the reply to email.
        /// </summary>
        /// <value>
        /// The reply to email.
        /// </value>
        string ReplyToEmail { get; set; }

        /// <summary>
        /// Gets or sets the cc emails.
        /// </summary>
        /// <value>
        /// The cc emails.
        /// </value>
        string CCEmails { get; set; }

        /// <summary>
        /// Gets or sets the BCC emails.
        /// </summary>
        /// <value>
        /// The BCC emails.
        /// </value>
        string BCCEmails { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the message meta data.
        /// </summary>
        /// <value>
        /// The message meta data.
        /// </value>
        string MessageMetaData { get; set; }

        /// <summary>
        /// Gets or sets the email attachment binary file ids.
        /// </summary>
        /// <value>
        /// The email attachment binary file ids.
        /// </value>
        IEnumerable<int> EmailAttachmentBinaryFileIds { get; }

        #endregion

        #region SMS Properties

        /// <summary>
        /// Gets or sets from number.
        /// </summary>
        /// <value>
        /// From number.
        /// </value>
        [Obsolete( "Use SmsFromSystemPhoneNumberId instead." )]
        [RockObsolete( "1.15" )]
        int? SMSFromDefinedValueId { get; set; }

        /// <summary>
        /// Gets or sets the system phone number identifier to use when sending an SMS message.
        /// </summary>
        /// <value>
        /// The system phone number identifier to use when sending an SMS message.
        /// </value>
        int? SmsFromSystemPhoneNumberId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string SMSMessage { get; set; }

        #endregion

        #region Push Notification Properties

        /// <summary>
        /// Gets or sets from number.
        /// </summary>
        /// <value>
        /// From number.
        /// </value>
        string PushTitle { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string PushMessage { get; set; }

        /// <summary>
        /// Gets or sets from number.
        /// </summary>
        /// <value>
        /// From number.
        /// </value>
        string PushSound { get; set; }

        /// <summary>
        /// Gets or sets the push image binary file identifier.
        /// </summary>
        /// <value>
        /// The push image binary file identifier.
        /// </value>
        int? PushImageBinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the push open action.
        /// </summary>
        /// <value>
        /// The push open action.
        /// </value>
        PushOpenAction? PushOpenAction { get; set; }

        /// <summary>
        /// Gets or sets the push open message.
        /// </summary>
        /// <value>
        /// The push open message.
        /// </value>
        string PushOpenMessage { get; set; }

        /// <summary>
        /// Gets or sets the push open message structured content JSON.
        /// </summary>
        /// <value>
        /// The push open message structured content JSON.
        /// </value>
        string PushOpenMessageJson { get; set; }

        /// <summary>
        /// Gets or sets the push data.
        /// </summary>
        /// <value>
        /// The push data.
        /// </value>
        string PushData { get; set; }

        /// <summary>
        /// Gets or sets the SMS attachment binary file ids.
        /// </summary>
        /// <value>
        /// The SMS attachment binary file ids.
        /// </value>
        IEnumerable<int> SMSAttachmentBinaryFileIds { get; }

        #endregion

    }
}
