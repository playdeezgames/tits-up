﻿Imports SPLORR.Game
Imports TU.Persistence

Friend Module MapInitializer
    Friend Sub Initialize(world As IWorld)
        For Each mapType In MapTypes.All
            Dim descriptor = mapType.ToMapTypeDescriptor
            InitializeMap(world, mapType, descriptor)
        Next
    End Sub
    Private Sub InitializeMap(world As IWorld, mapType As String, descriptor As MapTypeDescriptor)
        Dim map = world.CreateMap(mapType, descriptor.Size, descriptor.DefaultTerrainType)
        descriptor.CustomInitializer.Invoke(map)
        InitializeCells(map)
        PopulateCharacters(map, descriptor.SpawnCharacters)
        PopulateItems(map, descriptor.SpawnItems)
        descriptor.PostProcessor.Invoke(map)
    End Sub

    Private Sub InitializeCells(map As IMap)
        For Each cell In map.Cells
            cell.TerrainType.ToTerrainTypeDescriptor.CellInitializer.Invoke(cell)
        Next
    End Sub

    Private Sub PopulateItems(map As IMap, spawnItems As IReadOnlyDictionary(Of String, Integer))
        For Each entry In spawnItems
            Dim itemType = entry.Key
            Dim count = entry.Value
            While count > 0
                PopulateItem(map, itemType)
                count -= 1
            End While
        Next
    End Sub

    Private Sub PopulateItem(map As IMap, itemType As String)
        Dim candidate = RNG.FromEnumerable(map.Cells.Where(Function(x) x.IsTenable))
        Dim item = CreateItem(map.World, itemType)
        candidate.AddItem(item)
    End Sub

    Private Sub PopulateCharacters(map As IMap, spawnCharacters As IReadOnlyDictionary(Of String, Integer))
        For Each entry In spawnCharacters
            Dim characterType = entry.Key
            Dim count = entry.Value
            While count > 0
                PopulateCharacter(map, characterType)
                count -= 1
            End While
        Next
    End Sub

    Private Sub PopulateCharacter(map As IMap, characterType As String)
        Dim candidate = RNG.FromEnumerable(map.Cells.Where(Function(x) x.IsTenable AndAlso x.Character Is Nothing))
        CreateCharacter(characterType, candidate)
    End Sub

    Private Function CreateCharacter(characterType As String, candidate As ICell) As ICharacter
        candidate.Character = candidate.Map.World.CreateCharacter(characterType, candidate)
        Dim descriptor = characterType.ToCharacterTypeDescriptor
        With candidate.Character
            For Each entry In descriptor.Statistics
                .Statistic(entry.Key) = entry.Value
            Next
        End With
        descriptor.Initializer.Invoke(candidate.Character)
        Return candidate.Character
    End Function
End Module
