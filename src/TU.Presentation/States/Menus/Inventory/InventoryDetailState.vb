Imports TU.Persistence

Friend Class InventoryDetailState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Inventory)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case DropText
                SetState(GameState.Drop)
            Case EquipText
                SetState(GameState.Equip)
            Case Else
                If Game.DoItemVerb(value.Item2) Then
                    SetStates(GameState.Message, GameState.InventoryDetail)
                Else
                    OnStart()
                End If
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = $"{Game.ItemName}(x{Game.ItemCountByName(Game.ItemName)})"
        Dim result As New List(Of (String, String)) From {
            (DropText, DropText)
        }
        If Game.CanEquipItem Then
            result.Add((EquipText, EquipText))
        End If
        result.AddRange(Game.VerbTypesByItemName(Game.ItemName))
        Return result
    End Function

    Public Overrides Sub OnStart()
        If Game.ItemCountByName(Game.ItemName) = 0 Then
            SetState(GameState.Inventory)
            Return
        End If
        MyBase.OnStart()
    End Sub
End Class
