Friend Module RecipeTypes
    Friend ReadOnly Descriptors As IReadOnlyList(Of RecipeDescriptor) =
        New List(Of RecipeDescriptor) From
        {
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Rock, 2}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Rock, 1},
                    {ItemTypes.SharpRock, 1}
                }),
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.SharpRock, 1},
                    {ItemTypes.SnekCorpse, 1}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.SharpRock, 1},
                    {ItemTypes.Meat, 1},
                    {ItemTypes.Skin, 1}
                }),
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Fiber, 2}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Twine, 1}
                }),
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Twine, 2},
                    {ItemTypes.Moss, 1}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Bandage, 1}
                }),
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Twine, 1},
                    {ItemTypes.Stick, 1},
                    {ItemTypes.SharpRock, 1}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Axe, 1}
                }),
            New RecipeDescriptor(
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Twine, 3},
                    {ItemTypes.Stick, 3},
                    {ItemTypes.Skin, 10}
                },
                New Dictionary(Of String, Integer) From
                {
                    {ItemTypes.Shield, 1}
                })
        }
End Module
