Imports System.Drawing
Imports System.IO
Imports System.Text.Json
Imports AOS.UI

Module Program
    Const InputFilename = "monochrome_tilemap_packed.png"
    Const CellWidth = 8
    Const CellHeight = 8
    Sub Main(args As String())
        Dim bmp = New Bitmap(InputFilename)
        Dim rows = bmp.Height / CellHeight
        Dim columns = bmp.Width / CellWidth
        Dim glyph = " "c
        Dim fontData As New FontData
        fontData.Height = CellHeight
        fontData.Glyphs = New Dictionary(Of Char, GlyphData)
        For row = 0 To rows - 1
            For column = 0 To columns - 1
                Dim glyphData As New GlyphData With {.Width = CellWidth, .Lines = New Dictionary(Of Integer, IEnumerable(Of Integer))}
                fontData.Glyphs(glyph) = glyphData
                glyph = ChrW(AscW(glyph) + 1)
                Console.WriteLine(AscW(glyph))
                For y = 0 To CellHeight - 1
                    Dim line As New List(Of Integer)
                    For x = 0 To CellWidth - 1
                        Dim color = bmp.GetPixel(column * CellWidth + x, row * CellHeight + y)
                        If color.R < 128 Then
                            Console.Write(" ")
                        Else
                            Console.Write("#")
                            line.Add(x)
                        End If
                    Next
                    If line.Any Then
                        glyphData.Lines(y) = line
                    End If
                    Console.WriteLine()
                Next
            Next
        Next
        File.WriteAllText("starve.json", JsonSerializer.Serialize(fontData))
    End Sub
End Module
