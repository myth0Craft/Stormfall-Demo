# **Stormfall**

## Gameplay Demo Video


[![Stormfall Demo - First 3 Minutes of Gameplay](https://github.com/user-attachments/assets/cadc0753-4ad6-4c50-a507-8dd2b8b24aa9)](https://youtu.be/ekKG5TU68Wg)
https://youtu.be/ekKG5TU68Wg

## About the Game

Stormfall is a 2D metroidvania action-platformer set in a fallen kingdom. 
Reborn from an ancient slumber, a lone knight must embark on an epic journey across a kingdom 
of light and shadow. Restore the lost lights of the kingdom and master tight, fast-paced combat to save it from crumbling into a 
forgotten darkness.

## Play the Demo now on Itch.io: https://myth0.itch.io/stormfall
Available for Windows, Mac, and Linux.
<br>
Note: Mac and Linux builds are experimental. Issues may occur.
<br>
<br>
For all platforms, please report bugs here: 
<br>
https://forms.gle/FMkt3Zu89ZpSYcC9A
<br>
<br>
For generic feedback on the game's design, art style, or anything else, 
you can fill out this form: 
<br>
https://forms.gle/NS4dyosp4XfVnj2N6

## Controls
**Move Left:** A
<br>
**Move Right:** D
<br>
**Jump:** Space 
<br>
__Attack (requires sword ability):__ Left Click
<br>
**Interact:** Right Click
<br>
**Pause:** Esc

## Gallery
<img width="1919" height="1079" alt="Screenshot 2026-02-20 182508" src="https://github.com/user-attachments/assets/e7c11099-6a99-403c-961a-101f75ae56cc" />
<img width="1919" height="1079" alt="Screenshot 2026-03-26 095411" src="https://github.com/user-attachments/assets/5ee25c98-f6da-4a8b-8644-e827d70cc75e" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125802" src="https://github.com/user-attachments/assets/7dbe49ce-85c4-4ddf-9cbf-bdf3e853d467" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125244" src="https://github.com/user-attachments/assets/f5acfaec-e963-4f94-aeb1-4472991c6832" />
<img width="1919" height="1079" alt="Screenshot 2026-03-22 125231" src="https://github.com/user-attachments/assets/602d7f36-12bb-4ea4-962f-863c3cc2d1dd" />
<img width="1919" height="1079" alt="Screenshot 2026-03-18 183526" src="https://github.com/user-attachments/assets/c096a302-d41e-4e39-a464-4805d4a5c83f" />
<img width="1919" height="1079" alt="Screenshot 2026-03-14 103537" src="https://github.com/user-attachments/assets/62a8ae65-102e-462d-a12b-f24c61d39c21" />


## Credits
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

## What I Learned
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
<br>
<br>
<img width="1403" height="705" alt="Screenshot 2025-10-21 133331" src="https://github.com/user-attachments/assets/296b96c8-07f2-4183-b4fd-5d8cacad4452" />

I also had to spend several weeks at this stage of the project building my rendering system. 
Unity doesn't come with built in 2D blur effects, which turned out to be a major problem
because I wanted a depth of field effect with a blurred background and foreground. To achieve the effect, I had to write a custom
Gaussian weighted shader using Unity shader graphs, and a custom Render Feature to inject
into the rendering pipeline. The Render Feature was super complex and not very well-documented for Unity 6,
so it was super frusturating to get working. My initial setup had two extra scene cameras rendering to render textures.
The render textures were then passed into my render feature where the blur shader was applied to them, and then they got blitted back onto the screen to form
the background and foreground.
<br>
<br>
I spent a few weeks refining my art style. I learned the basics of 
2D animation, making readable sprites, and how to do game art efficiently. Every time the
art started to burn me out, which was often, I switched over to coding for a few days and made a few more
game mechanics. 
<br>
<br>
I also ran into a problem doing the player's animations. The player had to be able to play the walk animation and the attack animation at the same time.
However, this is near impossible to set up with one sprite alone because I would have had to make a seperate attack animation starting on every single frame of the walk animation, so the walk
animation didn't get cut off. In the end, I switched to a layer-based player sprite. Each part of the player, like the body, cape, sword, arms, and legs are all controlled independently by 
seperate animation controllers. Then they are layered on top of each other in the scene to form the full player body. 
<br>
<br>
After sorting out the animations and making a few high quality sprites, the game ended up looking something like this:
<br>
<br>
<img width="1919" height="1079" alt="Screenshot 2025-12-19 141051" src="https://github.com/user-attachments/assets/547f563b-5a7f-48b0-a5e2-4b93d392c48e" />

I also added my save/load system, the file select screen, basic enemies, a basic combat system, and save points to the game at around this time. 
<br>
<br>
By this point, I had coded most of the basic mechanics and gotten the art style down, so I started building the actual world level design. This is what the world looked like initially:
<br>
<br>
<img width="1126" height="566" alt="Screenshot 2026-01-28 163544" src="https://github.com/user-attachments/assets/0312a450-0235-4461-aff0-0c06a6a81a8b" />

Slowly, over the weeks and months I expanded the world and filled in the game's art. The current world map looks more like this: 
<br>
<br>
<img width="1036" height="713" alt="Screenshot 2026-03-30 085520" src="https://github.com/user-attachments/assets/4481ec23-f521-494b-afe0-e21104295ab6" />
At one point in this process, I updated my art style again to be more bloomy and saturated. I also completely redid my combat system by making it combo-based and improving the animations.
<br>
<br>
But even though the game demo itself was mostly done, my blur rendering system from earlier still had room for improvement. It was very unoptimized, annoying to set up, and all around not that great. 
So I decided to rewrite it. I made it so blur cameras were all created and rendered internally in a more optimized way. I implemented downsampling and fixed the UI for setting up the render feature. Now users 
simply have to add the feature to the renderer asset, select which layers to blur, and it does everything internally.
<br>
<br>
Building this short, 3 minute vertical slice was a long, complicated process. When I started, I never expected it to take this long for only a few minutes of gameplay. 
I now have a solid foundation to build from, though. I simply need to keep iterating on it and building more content.
I plan to keep on working on the project until I get bored of it or finish it. Who knows, maybe in a year, or a few, you'll see this game on Steam. 
<br>
<br>
If you read this far, thanks for reading. I hope you enjoy the Stormfall Demo.





