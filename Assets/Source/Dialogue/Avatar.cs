namespace Source.Dialogue
{
    /// <summary>
    /// List of character avatars that can be used in dialogue files.
    /// </summary>
    public enum Avatar
    {
        None = 0,
        Cyber = 1,
        Socket = 2,
    }
    
    /*
        Adding a new Avatar:
        
        1. Add new AvatarID to the enum, with specific unique value set 
        2. go to Unity > Assets > Animations > UI > DialogueAvatar
        3. Create a new animation, named '{AvatarEnumName}Speak' for the speaking animation.
        4. Create a new animation, named '{AvatarEnumName}Idle' for their non-speaking animation
        5. In the animator controller, route the parameters following existing patterns, i.e. Any -> Speaking -> Idle
     */
}