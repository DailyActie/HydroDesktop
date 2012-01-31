﻿Imports HydroDesktop.Database
Imports System.Globalization
Imports System.Threading
Imports System.Text
Imports HydroDesktop.Interfaces


Public Class fChangeYValue

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

    Public Sub New()

        InitializeComponent()
        ddlMethod.SelectedItem = ddlMethod.Items(0)

    End Sub

    Public Sub initialize()


    End Sub

    Private Sub btnApplyChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyChange.Click
        Dim changed As Boolean = False

        'Validation
        If Not IsNumeric(txtValue.Text) Then
            MsgBox("Please enter numbers")
            Exit Sub
        End If

        'Call Function
        'If _cEditView.pTimeSeriesPlot.HasEditingCurve Then
        '    Select Case ddlMethod.SelectedIndex
        '        Case 0
        '            _cEditView.pTimeSeriesPlot.ChangeValueByAddOrMinus(True, txtValue.Text)
        '            changed = True
        '        Case 1
        '            _cEditView.pTimeSeriesPlot.ChangeValueByAddOrMinus(False, txtValue.Text)
        '            changed = True
        '        Case 2
        '            _cEditView.pTimeSeriesPlot.ChangeValueByMultiply(txtValue.Text)
        '            changed = True
        '        Case 3
        '            _cEditView.pTimeSeriesPlot.ChangeValueBySettingValue(txtValue.Text)
        '            changed = True
        '    End Select
        'End If
        Select Case ddlMethod.SelectedIndex
            Case 0
                _cEditView.ChangeValueByAddOrMinus(True, txtValue.Text)
                changed = True
            Case 1
                _cEditView.ChangeValueByAddOrMinus(False, txtValue.Text)
                changed = True
            Case 2
                _cEditView.ChangeValueByMultiply(txtValue.Text)
                changed = True
            Case 3
                _cEditView.ChangeValueBySettingValue(txtValue.Text)
                changed = True
        End Select


        'update Edit View
        If changed Then
            'If _cEditView.pTimeSeriesPlot.HasEditingCurve Then
            '    _cEditView.ReflectChanges()
            'End If
            _cEditView.RefreshDataGridView()
            _cEditView.pTimeSeriesPlot.ReplotEditingCurve(_cEditView)
            Me.Close()
        End If


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub Leaving() Handles Me.Deactivate
        'Me.Close()
    End Sub

End Class