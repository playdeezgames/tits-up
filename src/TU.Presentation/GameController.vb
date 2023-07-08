Imports AOS.UI

Public Class GameController
    Inherits BaseGameController(Of IGameContext)

    Public Sub New(settings As ISettings, context As IUIContext(Of IGameContext))
        MyBase.New(settings, context)
        SetState(BoilerplateState.Embark, New EmbarkState(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Neutral, New NeutralState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Navigation, New NavigationState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Dead, New DeadState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Combat, New CombatState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Run, New RunState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Attack, New AttackState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Message, New MessageState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.ActionMenu, New ActionMenuState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Inventory, New InventoryState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Status, New StatusState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Ground, New GroundState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Take, New TakeState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.InventoryDetail, New InventoryDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Drop, New DropState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Interact, New InteractState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equip, New EquipState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equipment, New EquipmentState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.EquipmentDetail, New EquipmentDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Unequip, New UnequipState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Craft, New CraftState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Forage, New ForageState(Me, AddressOf SetCurrentState, context))
        SetCurrentState(BoilerplateState.Splash, True)
    End Sub
End Class
