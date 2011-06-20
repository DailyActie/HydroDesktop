﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using System.ComponentModel;

namespace HydroDesktop.Interfaces
{
    public interface ISeriesSelector
    {
        /// <summary>
        /// Get the array of all checked series IDs
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int[] CheckedIDList { get; }

        /// <summary>
        /// Get the array of all visible series IDs as defined by the current filter
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int[] VisibleIDList { get; }

        /// <summary>
        /// Get the currently selected (highlighted) series ID. If no series is selected, 
        /// 0 is returned.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int SelectedSeriesID { get; set; }

        /// <summary>
        /// Get or set the currently used filter expression to limit the number of displayed
        /// series. If there is no filter expression used, returns a empty string
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        string FilterExpression { get; set; }

        /// <summary>
        /// Get the currently used type of filter (All, Simple, Complex)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        FilterTypes FilterType { get; }

        /// <summary>
        /// Get the context menu that appears on right-click of a series
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ContextMenuStrip ContextMenuStrip { get; }

        /// <summary>
        /// When set to true, the series check boxes are visible. When set to false, the series check
        /// boxes are not visible.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool CheckBoxesVisible { get; set; }

        /// <summary>
        /// Refresh all check boxes according to the status of the database. This method should be
        /// called when the data repository database is changed by a different application
        /// </summary>
        void RefreshSelection();

        void SetupDatabase();

        /// <summary>
        /// When a series is checked or unchecked
        /// </summary>
        event SeriesEventHandler SeriesCheck;

        /// <summary>
        /// When a series is selected (highlighted)
        /// </summary>
        event EventHandler SeriesSelected;

        /// <summary>
        /// When the refresh method is called or the 'Refresh' button is pressed
        /// </summary>
        event EventHandler Refreshed;
    }
}
