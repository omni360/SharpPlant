﻿//
//  Copyright © 2013 Parrish Husband (parrish.husband@gmail.com)
//  The MIT License (MIT) - See LICENSE.txt for further details.
//

using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpPlant.SharpPlantReview
{
    /// <summary>
    ///     Provides the properties for creating a tag in SmartPlant Review.
    /// </summary>
    public class SprTag
    {
        #region Tag Properties

        /// <summary>
        ///     The parent Application reference.
        /// </summary>
        public SprApplication Application { get; private set; }

        /// <summary>
        ///     Holds the tag bitmask values used for tag placement.
        /// </summary>
        internal int Flags;

        /// <summary>
        ///     Indicates whether a leader will be present.
        /// </summary>
        public bool DisplayLeader
        {
            get
            {
                // Return the bitwise zero check
                return (Flags & SprConstants.SprTagLeader) != 0;
            }
            set
            {
                // Set flag true/false
                if (value) Flags |= SprConstants.SprTagLeader;
                else Flags &= ~SprConstants.SprTagLeader;
            }
        }

        /// <summary>
        ///     Point where the tag is displayed.  If the tag has not been placed, the point coordinates will be 0, 0, 0.
        /// </summary>
        public SprPoint3D OriginPoint
        {
            get
            {
                if (!IsPlaced) return new SprPoint3D(0, 0, 0);
                return new SprPoint3D(Convert.ToDouble(TagData["tag_origin_x"]),
                                   Convert.ToDouble(TagData["tag_origin_y"]),
                                   Convert.ToDouble(TagData["tag_origin_z"]));
            }
            set
            {
                if (!IsPlaced) return;
                TagData["tag_origin_x"] = value.East;
                TagData["tag_origin_y"] = value.North;
                TagData["tag_origin_z"] = value.Elevation;
            }
        }

        /// <summary>
        ///     Point for the end of the leader.  If the tag has not been placed, the point coordinates will be 0, 0, 0.
        /// </summary>
        public SprPoint3D LeaderPoint
        {
            get
            {
                if (!IsPlaced) return new SprPoint3D(0, 0, 0);
                return new SprPoint3D(Convert.ToDouble(TagData["tag_point_x"]),
                                   Convert.ToDouble(TagData["tag_point_y"]),
                                   Convert.ToDouble(TagData["tag_point_z"]));
            }
            set
            {
                if (!IsPlaced) return;
                TagData["tag_point_x"] = value.East;
                TagData["tag_point_y"] = value.North;
                TagData["tag_point_z"] = value.Elevation;
            }
        }

        /// <summary>
        ///     Tag text.
        /// </summary>
        public string Text
        {
            get { return TagData["tag_text"].ToString(); }
            set { TagData["tag_text"] = value; }
        }

        /// <summary>
        ///     Size of the tag bubble. If the tag has not been placed, the value is set to zero.
        /// </summary>
        public double Size
        {
            get { return IsPlaced ? Convert.ToDouble(TagData["tag_size"]) : 0; }
            set { if (IsPlaced) TagData["tag_size"] = value; }
        }

        /// <summary>
        ///     Date the tag was placed.  If the tag has not been placed, the value is N/A.
        /// </summary>
        public string DatePlaced
        {
            get { return IsPlaced ? TagData["date_placed"].ToString() : "N/A"; }
            internal set { if (IsPlaced) TagData["date_placed"] = value; }
        }

        /// <summary>
        ///     Date the tag was last edited.  If the tag has not been placed, the value is N/A.
        /// </summary>
        public string LastEdited
        {
            get { return IsPlaced ? TagData["last_edited"].ToString() : "N/A"; }
            internal set { if (IsPlaced) TagData["last_edited"] = value; }
        }

        /// <summary>
        ///     Color of the tag text.
        /// </summary>
        public Color TextColor
        {
            get { return SprUtilities.From0Bgr((int) TagData["number_color"]); }
            set { TagData["number_color"] = SprUtilities.Get0Bgr(value); }
        }

        /// <summary>
        ///     Color of the tag background.
        /// </summary>
        public Color BackgroundColor
        {
            get { return SprUtilities.From0Bgr((int) TagData["backgnd_color"]); }
            set { TagData["backgnd_color"] = SprUtilities.Get0Bgr(value); }
        }

        /// <summary>
        ///     Color of the tag leader line.
        /// </summary>
        public Color LeaderColor
        {
            get { return SprUtilities.From0Bgr((int) TagData["leader_color"]); }
            set { TagData["leader_color"] = SprUtilities.Get0Bgr(value); }
        }

        /// <summary>
        ///     Discipline the tag is set to.
        /// </summary>
        public string Discipline
        {
            get { return TagData["discipline"].ToString(); }
            set { TagData["discipline"] = value; }
        }

        /// <summary>
        ///     Author of the tag.
        /// </summary>
        public string Creator
        {
            get { return TagData["creator"].ToString(); }
            set { TagData["creator"] = value; }
        }

        /// <summary>
        ///     Computer the tag was created on.
        /// </summary>
        public string ComputerName
        {
            get { return TagData["computer_name"].ToString(); }
            set { TagData["computer_name"] = value; }
        }

        /// <summary>
        ///     Status of the tag.
        /// </summary>
        public string Status
        {
            get { return TagData["status"].ToString(); }
            set { TagData["status"] = value; }
        }

        /// <summary>
        ///     Determines if the tag has been placed in SmartPlant Review.
        /// </summary>
        public bool IsPlaced { get; internal set; }

        /// <summary>
        ///     Tag unique identification number.
        /// </summary>
        public int TagNumber
        {
            get { return Convert.ToInt32(TagData["tag_unique_id"]); }
            private set { TagData["tag_unique_id"] = value; }
        }

        /// <summary>
        ///     The full information profile of the current tag.  Controls all the tag properties.
        /// </summary>
        public Dictionary<string, object> TagData { get; set; }

        #endregion

        // Tag constructor
        public SprTag()
        {
            // Link the parent application
            Application = SprApplication.ActiveApplication;

            // Set as not placed by default
            IsPlaced = false;

            // Create a new data dictionary from the template
            TagData = SprUtilities.TagTemplate;

            // Set the tag to the next available tag number
            TagNumber = Application.NextTag;

            // Set the leader display by default
            DisplayLeader = true;

            // Add the default data values
            BackgroundColor = SprUtilities.From0Bgr(12632319);
            LeaderColor = SprUtilities.From0Bgr(12632319);

            // Set the tag creator
            Creator = Environment.GetEnvironmentVariable("USERNAME");

            // Set the computer name
            ComputerName = Environment.GetEnvironmentVariable("COMPUTERNAME");

            // Set the default status
            Status = "Open";
        }
    }
}