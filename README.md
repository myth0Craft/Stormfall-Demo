# **Stormfall**

## Gameplay Demo


[![Stormfall Demo - First 3 Minutes of Gameplay](https://img.youtube.com/vi/ekKG5TU68Wg/maxresdefault.jpg)](https://youtu.be/ekKG5TU68Wg)

## About the Game

Stormfall is a 2D metroidvania action-platformer set in a fallen kingdom. 
Reborn from an ancient slumber, a lone knight must embark on an epic journey across a kingdom 
of light and shadow. Restore the lost lights of the kingdom and save the world from a 
forgotten darkness.

## Play the Demo now on Itch.io: https://myth0.itch.io/stormfall
Available for Windows, Mac, and Linux.
\
Note: Mac and Linux builds are experimental. Issues may occur.
<br>
<br>
For all platforms, please report bugs here: 
\
https://forms.gle/FMkt3Zu89ZpSYcC9A
<br>
<br>
For generic feedback on the game's design, art style, or anything else, 
you can fill out this form: \
https://forms.gle/NS4dyosp4XfVnj2N6

### Controls
**Move Left:** A
\
**Move Right:** D
\
**Jump:** Space 
\
__Attack (requires sword ability):__ Left Click
\
**Interact:** Right Click
\
**Pause:** Esc

# Gallery
<img width="1919" height="1079" alt="Screenshot 2026-03-26 095411" src="https://github.com/user-attachments/assets/5ee25c98-f6da-4a8b-8644-e827d70cc75e" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125802" src="https://github.com/user-attachments/assets/7dbe49ce-85c4-4ddf-9cbf-bdf3e853d467" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125244" src="https://github.com/user-attachments/assets/f5acfaec-e963-4f94-aeb1-4472991c6832" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125231" src="https://github.com/user-attachments/assets/602d7f36-12bb-4ea4-962f-863c3cc2d1dd" />
<img width="1919" height="1079" alt="Screenshot 2026-03-18 183526" src="https://github.com/user-attachments/assets/c096a302-d41e-4e39-a464-4805d4a5c83f" />
<img width="1919" height="1079" alt="Screenshot 2026-03-14 103537" src="https://github.com/user-attachments/assets/62a8ae65-102e-462d-a12b-f24c61d39c21" />
<img width="1919" height="1079" alt="Screenshot 2026-02-20 182508" src="https://github.com/user-attachments/assets/e7c11099-6a99-403c-961a-101f75ae56cc" />

# Credits
All gameplay, art, music, and code were created and designed by me.
<br>
<br>
Code is written in Visual Studio 2022. The game is built in Unity Engine 6.3 using
the Universal Render Pipeline. I hand-drew all art in Krita. I composed the music in
Cakewalk Sonar.
<br>
<br>
Sound effects credit: https://freesound.org/

<!--# Why I Made This
I've always loved 2D games. I grew up playing the Ori games on the Xbox One and 
Super Mario Bros on the Wii.
I've also always loved programming. From a young age I was obsessed with making 
addons and mods for Minecraft. Over the last year or two, I've also become a Hollow Knight
superfan. So you can imagine I was pretty excited when Silksong got announced. 
<br>
<br>
When Silksong came out, I played it and enjoyed it. But I kept thinking to myself, 
"Man, it would be so fun to design my own massive 2D world for players to explore."
It started with drawing out maps of the game's kingdom. Then I started researching engines, 
tools, and digital art basics.
And now here we are. I've released a full demo of the first few minutes of the game.-->

# What I Learned
When I first started, I had never touched a proper game engine before, so it took a while
to learn the basics of Unity and scripting. I had also never done any digital art before
and had no idea where to start.
<br>
<br>
I started out with coding the player movement system. I set up a simple script and 
tweaked the values until I got it
feeling good. However, that was the easy part. Then I had to
work on the hard part, the art. So I drew out a few first basic sprites in Krita.
It initially looked something like this:

I also
had to spend several weeks at this stage of the project building my rendering system. 
Unity doesn't come with built in 2D blur effects, which turned out to be a major problem
because I wanted a depth of field effect with a blurred background and foreground. Hollow
Knight and Ori both have an effect like this. To achieve the effect, I had to write a custom
Gaussian weighted shader using Unity shader graphs, and a custom Render Feature to inject
into the rendering pipeline. The Render Feature was super complex and not very well-documented,
so it was super frusturating to get working. But I got it in the end after weeks of suffering.
<br>
<br>
I spent maybe about a month and a half refining my art style. I learned the basics of 
2D animation, making readable sprites, and how do do game art efficiently. Every time the
art started to burn me out, which was often, I switched over to coding and made a few new
game mechanics.





