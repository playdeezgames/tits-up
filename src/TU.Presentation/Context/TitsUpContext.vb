Imports System.IO

Public Class TitsUpContext
    Inherits UIContext(Of IGameContext)

    Public Sub New(fontFilenames As IReadOnlyDictionary(Of String, String), viewSize As (Integer, Integer))
        MyBase.New(New GameContext, fontFilenames, viewSize)
    End Sub

    Public Overrides ReadOnly Property AvailableWindowSizes As IEnumerable(Of (Integer, Integer))
        Get
            Return {
                (ViewWidth * 3, ViewHeight * 3),
                (ViewWidth * 5, ViewHeight * 5),
                (ViewWidth * 8, ViewHeight * 8),
                (ViewWidth * 10, ViewHeight * 10),
                (ViewWidth * 13, ViewHeight * 13),
                (ViewWidth * 15, ViewHeight * 15),
                (ViewWidth * 18, ViewHeight * 18),
                (ViewWidth * 20, ViewHeight * 20)
                }
        End Get
    End Property

    Public Overrides Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font)
        Dim text = "Tits Up!!"
        Dim x = ViewWidth \ 2 - font.TextWidth(text) \ 2
        Dim y = ViewHeight \ 2 - font.Height \ 2
        With font
            .WriteText(displayBuffer, (x + 1, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x + 1, y), text, Hue.Pink)
            .WriteText(displayBuffer, (x + 1, y + 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y + 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x, y + 1), text, Hue.Pink)

            .WriteText(displayBuffer, (x, y), text, Hue.Red)
        End With
        ShowStatusBar(displayBuffer, font, "Space/(A) - Continue", Hue.Black, Hue.LightGray)
    End Sub

    Public Overrides Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font)
        With font
            .WriteText(displayBuffer, (0, 0), "About Tits Up!!", Hue.Orange)
            .WriteText(displayBuffer, (0, font.Height * 2), "Art:", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 3), "https://vurmux.itch.io/urizen-onebit-tileset", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 5), "A Production of TheGrumpyGameDev", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 7), "For Weeksauce (July 2023)", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 9), "See 'aboot.txt'", Hue.White)
        End With
    End Sub

    Public Overrides Sub AbandonGame()
        Game.Abandon()
    End Sub

    Public Overrides Sub LoadGame(slot As Integer)
        Game.Load(SlotFilename(slot))
    End Sub

    Public Overrides Sub SaveGame(slot As Integer)
        Game.Save(SlotFilename(slot))
    End Sub

    Public Overrides Function DoesSlotExist(slot As Integer) As Boolean
        Return File.Exists(SlotFilename(slot))
    End Function

    Private ReadOnly SlotFilename As IReadOnlyDictionary(Of Integer, String) =
        New Dictionary(Of Integer, String) From
        {
            {1, "slot1.json"},
            {2, "slot2.json"},
            {3, "slot3.json"},
            {4, "slot4.json"},
            {5, "slot5.json"}
        }
End Class
