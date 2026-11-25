# Lab 08

**Course:** CS-464 Game Development (ITU)  
**Name:** Mustafa Fawwaz  
**Roll No.:** BSSE23036  
**Video Link:** 


## Overview

This repository extends the **Rope Hero: Vice Arena** UI project into a multi-scene, audio-driven playable prototype. It implements a 4-channel `AudioMixer` with logarithmic volume persistence, persistent singleton audio managers (`UIAudioPlayer`, pooled `SfxPlayer`, and equal-power crossfading `MusicPlayer`), an asynchronous scene loader with real progress tracking, and a decoupled C# event architecture (`HeroHealth`) driving HUD, audio, VFX, and camera feedback.

## Scene Flow & Build Profiles

Both scenes are registered in **File → Build Profiles → Scene List**:

| Index | Scene           | Role                                                                                                |
| :---: | :-------------- | :-------------------------------------------------------------------------------------------------- |
|  `0`  | `MainMenuScene` | Title, Main Menu, Mode Select, Level Select, Loadout, Shop, Settings, Credits, and `LoadingScreen`  |
|  `1`  | `GameplayScene` | Playable 3D Vice Arena patrol, collectibles, hazard zone, Pause/Settings modals, and Results screen |

### Asynchronous Loading (`SceneLoader.cs`)

- Selecting **DEPLOY** on `LevelSelectScreen` opens `LoadingScreen` and calls `SceneManager.LoadSceneAsync("GameplayScene")`.
- Real load progress is normalized via `Mathf.Clamp01(op.progress / 0.9f)` and displayed on a `Filled` progress bar and percentage readout.
- `allowSceneActivation` is held at `false` until both background loading and visual progress reach `100%`, preventing premature gameplay entry.

## Audio Architecture

### 1. 4-Group Audio Mixer (`GameMixer`) & Persistence (`AudioMixerManager.cs`)

All audio sources route through **`GameMixer`** with exposed attenuation parameters persisted via `PlayerPrefs`:

| Mixer Group | Exposed Parameter |     Default     | Behavior                                                                     |
| :---------- | :---------------- | :-------------: | :--------------------------------------------------------------------------- |
| **Master**  | `MasterVolume`    | `100%` (`0 dB`) | Global output level                                                          |
| **Music**   | `MusicVolume`     |      `80%`      | Looping background tracks (`ignoreListenerPause = true`)                     |
| **SFX**     | `SfxVolume`       |      `85%`      | Gameplay effects; suspends automatically when `AudioListener.pause = true`   |
| **UI**      | `UiVolume`        |      `90%`      | Menu/modal feedback (`ignoreListenerPause = true` so Pause UI stays audible) |

Slider values $v \in [0.0001, 1.0]$ convert logarithmically to decibels:

$$\text{dB} = \log_{10}(v) \times 20$$

### 2. Persistent Audio Singletons (`PersistentAudioRig`)

- **`UIAudioPlayer.cs`:** Plays non-overlapping UI sounds via `PlayOneShot` (`Hover`, `Click`, `Back`, `Success`, `Error`, `Pause`, `Resume`, `PanelToggle`) driven by `UIButtonFeedback`, `UIManager`, `ShopScreenController`, and `LoadoutScreenController`.
- **`SfxPlayer.cs`:** Cycles through a pool of 6 `AudioSource` voices routed to `SFX` with $\pm 2$ semitone pitch jitter ($2^{\text{semitones}/12}$) and subtle volume variation (`0.85–1.0`) so rapid pickups and abilities layer cleanly without clipping or interrupting one another.
- **`MusicPlayer.cs`:** Manages two 2D looping `AudioSource` channels (`Menu`, `Patrol`, `Combat`, `Results`) using an equal-power crossfade curve ($\cos(u \cdot \frac{\pi}{2})$ out, $\sin(u \cdot \frac{\pi}{2})$ in) to avoid mid-transition volume dips.

## Event-Driven Gameplay Architecture (C# Events)

Damage and defeat logic in `GameplayScene` is decoupled using standard C# `event Action` delegates on **`HeroHealth.cs`**:

- **Publisher (`HeroHealth.cs`):**
  - `event Action<int, int> HealthChanged` — broadcast on initialization, damage, and healing.
  - `event Action<int, int> Damaged` — broadcast whenever `HazardDamageTrigger` inflicts damage.
  - `event Action Died` — broadcast when `currentHp` reaches `0`.
- **Independent Subscribers** (subscribing with `+=` in `OnEnable()` and unsubscribing with `-=` in `OnDisable()`):
  1. **`ArmorHudListener.cs`:** Updates the HUD `ARMOR HP` filled bar and percentage readout.
  2. **`HeroDamageAudioListener.cs`:** Triggers hurt and defeat SFX through `SfxPlayer`.
  3. **`HitImpactVfxListener.cs`:** Plays the `HitSparks` burst particle system on `ViceHero`.
  4. **`CameraFeedbackListener.cs`:** Applies local camera shake inside `CameraRig`, flashes `DamageFlashOverlay`, and triggers `ResultsScreen` on `Died`.

## Gameplay Controls

| Action             | Keyboard Input           | HUD Button              |
| :----------------- | :----------------------- | :---------------------- |
| Move Hero          | `W A S D` / `Arrow Keys` | -                       |
| Rope Grapple Boost | `E`                      | `GRAPPLE (SWING)`       |
| Optic Laser Attack | `F`                      | `LASER BEAM`            |
| Pause / Resume     | `Escape`                 | `PAUSE \|\|` / `RESUME` |

## Audio Asset Credits & Licensing

All audio clips and background music tracks were downloaded from [**Freesound.org**](https://freesound.org) under **Creative Commons 0 (CC0)** and **Creative Commons Attribution 4.0 (CC BY 4.0)** licenses:

### UI Sound Effects (`Assets/Audio/UI/`)

- `Button_Hover (ogg).ogg` by **deadsillyrabbit** — _License: Creative Commons 0_
- `fidget toy` by **dyingSatyr** — _License: Creative Commons 0_
- `UI Button Sound (Cancel / Back / Exit)` by **Nomagician** — _License: Creative Commons 0_
- `UI Panel Slide In` by **Qvantor** — _License: Creative Commons 0_
- `Error` by **EilasSFX** — _License: Creative Commons 0_
- `Simple or Cute UI / UX / Interface Pause sound` by **Feraly\_** — _License: Creative Commons 0_
- `Resume` by **MATUSTRM** — _License: Creative Commons 0_
- `handful-of-coins-006` by **ilyaShevelev** — _License: Creative Commons 0_

### Gameplay Sound Effects (`Assets/Audio/SFX/`)

- `hook.wav` by **kfatehi** — _License: Creative Commons 0_
- `Laser03.2rev.wav` by **chipfork71** — _License: Attribution 4.0_
- `Item Pickup` by **TreasureSounds** — _License: Attribution 4.0_
- `male_hurt9.ogg` by **micahlg** — _License: Creative Commons 0_
- `result-2.mp3` by **DZeDeNZ** — _License: Creative Commons 0_
- `You Lose.mp3` by **grizzlymittz** — _License: Creative Commons 0_

### Background Music & Ambience (`Assets/Audio/Music/`)

- `video game menu music` by **magmadiverrr** — _License: Creative Commons 0_
- `shore amer river Netherlands in spring 947 am 250423_1077` by **klankbeeld** — _License: Attribution 4.0_
- `TensionMusicLoop` by **Akkaittou** — _License: Attribution 4.0_
- `pattern.aiff` by **wkalmar** — _License: Creative Commons 0_
- `whisper.wav` by **IanStarGem** — _License: Creative Commons 0_
