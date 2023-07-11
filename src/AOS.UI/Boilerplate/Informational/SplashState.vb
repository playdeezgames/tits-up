﻿Friend Class SplashState(Of TModel)
    Inherits BaseGameState(Of TModel)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A
                SetState(BoilerplateState.MainMenu)
        End Select
    End Sub
    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, BoilerplateHue.Black)
        Context.ShowSplashContent(displayBuffer, Context.Font(UIFont))
    End Sub
End Class
