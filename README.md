# Sprite Combinator

A desktop tool built with Unity that generates every unique combination from layered sprites. Point it at a folder of images per layer and it composites them into all possible variations — useful for NFT-style collections, avatars, game characters, sticker packs, or any layered art.

## How It Works

You add as many layers as you need, each pointing to a folder of PNG images. The tool composites one image from every layer, bottom to top, using alpha blending, and writes out every possible combination.

**Example:** 10 backgrounds × 8 bodies × 12 hats × 5 items = **4,800 unique images**

## Usage

1. Open the project in Unity (2020.3+)
2. Open `Assets/Scenes/Main Scene`
3. Press Play
4. Add a layer for each part of your art (**Add layer** / remove with **X**), and optionally name it
5. For each layer, **Select** the folder of PNG images
6. Pick the **Result folder** for the output
7. Set how many images to generate (up to the shown maximum)
8. Click **Generate**

Layers composite bottom to top: the first layer is the background (marked **bottom**), the last is drawn on top (**top**). The output resolution matches the first layer — any layer of a different size is scaled to fit it, so for best quality make the bottom layer full-size.

## Project Structure

```
Assets/Scripts/
├── Generator/
│   └── ImageGenerator.cs      # Combination + alpha-blend + scaling, writes PNGs
├── Helper/
│   ├── FolderPicker.cs        # Folder dialog (editor + standalone build)
│   ├── ShuffleHelper.cs       # Fisher-Yates shuffle
│   └── PathHelper.cs          # Directory file listing
└── UI/
    ├── MainScreen.cs          # Layer list, counts, generation flow
    ├── LayerEntry.cs          # One layer row (name, folder, order)
    ├── ProcessingScreen.cs    # Progress + result screen
    └── ProgressBar.cs         # Progress bar widget
```

## Features

- Add, remove, name and reorder any number of layers (up to a configurable limit)
- Generates all permutations of the layered images
- Auto-scales mismatched layer sizes to the bottom layer
- Randomizes combination order for variety
- Live max-count and progress tracking
- Memory-efficient batch processing with periodic cleanup
- Native folder dialog in the editor, standalone file browser in builds
- Dark UI theme

## Dependencies

- [StandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser) — included, used in standalone builds

## License

MIT
