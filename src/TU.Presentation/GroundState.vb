Friend Class GroundState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "On The Ground", context.ControlsText("Select", "Cancel"), Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Game.Avatar.GroundItems
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Game.Avatar.HasGroundItems Then
            SetState(Neutral)
        End If
    End Sub
End Class
