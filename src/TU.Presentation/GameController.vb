Imports AOS.UI

Public Class GameController
    Inherits BaseGameController(Of IGameContext)

    Public Sub New(settings As ISettings, context As IUIContext(Of IGameContext))
        MyBase.New(settings, context)
        SetState(BoilerplateState.Embark, New EmbarkState(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Neutral, New NeutralState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Navigation, New NavigationState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.MoveNorth, New MoveState(Me, AddressOf SetCurrentState, context, (0, -1)))
        SetState(GameState.MoveSouth, New MoveState(Me, AddressOf SetCurrentState, context, (0, 1)))
        SetState(GameState.MoveWest, New MoveState(Me, AddressOf SetCurrentState, context, (-1, 0)))
        SetState(GameState.MoveEast, New MoveState(Me, AddressOf SetCurrentState, context, (1, 0)))
        SetState(GameState.InteractCharacter, New InteractCharacterState(Me, AddressOf SetCurrentState, context))
        SetCurrentState(BoilerplateState.Splash, True)
    End Sub
End Class
