Friend Class ActionMenuState
    Inherits BasePickerState(Of IGameContext, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "Actions", context.ControlsText("Select", "Cancel"), BoilerplateState.Neutral)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case StatisticsText
                SetState(GameState.Statistics)
            Case GroundText
                SetState(GameState.Ground)
        End Select
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Dim result As New List(Of (String, String))
        If Game.Avatar.HasGroundItems Then
            result.Add((GroundText, GroundText))
        End If
        result.Add((StatisticsText, StatisticsText))
        Return result
    End Function
End Class
