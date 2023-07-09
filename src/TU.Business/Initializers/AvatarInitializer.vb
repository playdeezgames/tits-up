Imports TU.Persistence

Friend Module AvatarInitializer
    Friend Sub Initialize(world As IWorld)
        world.Avatar = world.Characters.Single(Function(x) x.CharacterType = CharacterTypes.Tizzy)
    End Sub
End Module
