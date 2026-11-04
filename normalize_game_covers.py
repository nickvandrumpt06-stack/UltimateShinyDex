from pathlib import Path
from PIL import Image, ImageFilter

INPUT_DIR = Path("frontend/public/game-covers")
OUTPUT_DIR = Path("frontend/public/game-covers-normalized")

CANVAS_WIDTH = 1200
CANVAS_HEIGHT = 800
PADDING = 40

OUTPUT_DIR.mkdir(exist_ok=True)

for image_path in INPUT_DIR.iterdir():
    if image_path.suffix.lower() not in [".png", ".jpg", ".jpeg", ".webp"]:
        continue

    image = Image.open(image_path).convert("RGB")

    # Create a blurred background that fills the canvas
    background = image.copy()
    background.thumbnail((CANVAS_WIDTH, CANVAS_HEIGHT))

    scale = max(
        CANVAS_WIDTH / background.width,
        CANVAS_HEIGHT / background.height
    )

    background = background.resize(
        (
            int(background.width * scale),
            int(background.height * scale)
        )
    )

    left = (background.width - CANVAS_WIDTH) // 2
    top = (background.height - CANVAS_HEIGHT) // 2

    background = background.crop(
        (
            left,
            top,
            left + CANVAS_WIDTH,
            top + CANVAS_HEIGHT
        )
    )

    background = background.filter(
        ImageFilter.GaussianBlur(25)
    )

    # Slightly darken / soften the background
    overlay = Image.new(
        "RGB",
        (CANVAS_WIDTH, CANVAS_HEIGHT),
        (255, 255, 255)
    )

    background = Image.blend(
        background,
        overlay,
        0.25
    )

    # Resize the real artwork to fit inside the canvas
    foreground = image.copy()

    foreground.thumbnail(
        (
            CANVAS_WIDTH - PADDING * 2,
            CANVAS_HEIGHT - PADDING * 2
        )
    )

    x = (CANVAS_WIDTH - foreground.width) // 2
    y = (CANVAS_HEIGHT - foreground.height) // 2

    background.paste(
        foreground,
        (x, y)
    )

    output_path = OUTPUT_DIR / f"{image_path.stem}.jpg"

    background.save(
        output_path,
        quality=92
    )

    print(f"Created: {output_path}")