const spriteBase =
  'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny'

function shinyById(id) {
  return `${spriteBase}/${id}.png`
}

function shinyForm(dexNumber, formName) {
  return `${spriteBase}/${dexNumber}-${formName}.png`
}

export function shinyFemale(dexNumber) {
  return `${spriteBase}/female/${dexNumber}.png`
}

export const formSprites = {
  // Kanto / regional forms
  'Rattata|Alolan': shinyById(10091),
  'Raticate|Alolan': shinyById(10092),

  'Pikachu|Partner Cap': shinyById(10148),

  'Raichu|Alolan': shinyById(10100),

  'Sandshrew|Alolan': shinyById(10101),
  'Sandslash|Alolan': shinyById(10102),

  'Vulpix|Alolan': shinyById(10103),
  'Ninetales|Alolan': shinyById(10104),

  'Diglett|Alolan': shinyById(10105),
  'Dugtrio|Alolan': shinyById(10106),

  'Meowth|Alolan': shinyById(10107),
  'Meowth|Galarian': shinyById(10161),
  'Persian|Alolan': shinyById(10108),

  'Growlithe|Hisuian': shinyById(10229),
  'Arcanine|Hisuian': shinyById(10230),

  'Geodude|Alolan': shinyById(10109),
  'Graveler|Alolan': shinyById(10110),
  'Golem|Alolan': shinyById(10111),

  'Ponyta|Galarian': shinyById(10162),
  'Rapidash|Galarian': shinyById(10163),

  'Slowpoke|Galarian': shinyById(10164),
  'Slowbro|Galarian': shinyById(10165),

  "Farfetch'd|Galarian": shinyById(10166),

  'Grimer|Alolan': shinyById(10112),
  'Muk|Alolan': shinyById(10113),

  'Voltorb|Hisuian': shinyById(10231),
  'Electrode|Hisuian': shinyById(10232),

  'Exeggutor|Alolan': shinyById(10114),

  'Marowak|Alolan': shinyById(10115),

  'Weezing|Galarian': shinyById(10167),

  'Tauros|Paldean Combat Breed': shinyById(10250),
  'Tauros|Paldean Blaze Breed': shinyById(10251),
  'Tauros|Paldean Aqua Breed': shinyById(10252),

  'Articuno|Galarian': shinyById(10169),
  'Zapdos|Galarian': shinyById(10170),
  'Moltres|Galarian': shinyById(10171),

  // Johto
  'Typhlosion|Hisuian': shinyById(10233),

  'Wooper|Paldean': shinyById(10253),

  'Slowking|Galarian': shinyById(10172),

  'Qwilfish|Hisuian': shinyById(10234),

  'Sneasel|Hisuian': shinyById(10235),

  'Corsola|Galarian': shinyById(10173),

  // Hoenn
  'Zigzagoon|Galarian': shinyById(10174),
  'Linoone|Galarian': shinyById(10175),

  'Deoxys|Attack Forme': shinyById(10001),
  'Deoxys|Defense Forme': shinyById(10002),
  'Deoxys|Speed Forme': shinyById(10003),

  // Sinnoh
  'Burmy|Sandy Cloak': shinyForm(412, 'sandy'),
  'Burmy|Trash Cloak': shinyForm(412, 'trash'),

  'Wormadam|Sandy Cloak': shinyById(10004),
  'Wormadam|Trash Cloak': shinyById(10005),

  'Shellos|East Sea': shinyForm(422, 'east'),
  'Gastrodon|East Sea': shinyForm(423, 'east'),

  'Rotom|Heat': shinyById(10008),
  'Rotom|Wash': shinyById(10009),
  'Rotom|Frost': shinyById(10010),
  'Rotom|Fan': shinyById(10011),
  'Rotom|Mow': shinyById(10012),

  'Shaymin|Sky Forme': shinyById(10006),

  // Unova
  'Samurott|Hisuian': shinyById(10236),

  'Lilligant|Hisuian': shinyById(10237),

  'Basculin|Blue-Striped': shinyById(10016),
  'Basculin|White-Striped': shinyById(10247),

  'Darumaka|Galarian': shinyById(10176),
  'Darmanitan|Galarian': shinyById(10177),

  'Yamask|Galarian': shinyById(10179),

  'Zorua|Hisuian': shinyById(10238),
  'Zoroark|Hisuian': shinyById(10239),

  'Stunfisk|Galarian': shinyById(10180),

  'Braviary|Hisuian': shinyById(10240),

  'Tornadus|Therian Forme': shinyById(10019),
  'Thundurus|Therian Forme': shinyById(10020),
  'Landorus|Therian Forme': shinyById(10021),

  'Keldeo|Resolute Form': shinyById(10024),

  // Kalos
  'Floette|Eternal Flower': shinyById(10061),

  'Sliggoo|Hisuian': shinyById(10241),
  'Goodra|Hisuian': shinyById(10242),

  'Pumpkaboo|Small Size': shinyById(10027),
  'Pumpkaboo|Large Size': shinyById(10028),
  'Pumpkaboo|Super Size': shinyById(10029),

  'Gourgeist|Small Size': shinyById(10030),
  'Gourgeist|Large Size': shinyById(10031),
  'Gourgeist|Super Size': shinyById(10032),

  'Avalugg|Hisuian': shinyById(10243),

  'Zygarde|10% Forme': shinyById(10181),

  'Hoopa|Unbound': shinyById(10086),

  // Alola
  'Decidueye|Hisuian': shinyById(10244),

  'Oricorio|Pom-Pom Style': shinyById(10123),
  "Oricorio|Pa'u Style": shinyById(10124),
  'Oricorio|Sensu Style': shinyById(10125),

  'Lycanroc|Midnight Form': shinyById(10126),
  'Lycanroc|Dusk Form': shinyById(10152),

  'Minior|Red Core': shinyById(10136),
  'Minior|Orange Core': shinyById(10137),
  'Minior|Yellow Core': shinyById(10138),
  'Minior|Green Core': shinyById(10139),
  'Minior|Blue Core': shinyById(10140),
  'Minior|Indigo Core': shinyById(10141),
  'Minior|Violet Core': shinyById(10142),

  // Galar / Hisui
  'Toxtricity|Low Key Form': shinyById(10184),

  'Sinistea|Antique Form': shinyForm(854, 'antique'),
  'Polteageist|Antique Form': shinyForm(855, 'antique'),

  'Urshifu|Rapid Strike Style': shinyById(10191),

  'Ursaluna|Bloodmoon': shinyById(10272),

  'Enamorus|Therian Forme': shinyById(10249),

  // Paldea
  'Maushold|Family of Three': shinyById(10257),

  'Squawkabilly|Blue Plumage': shinyById(10260),
  'Squawkabilly|Yellow Plumage': shinyById(10261),
  'Squawkabilly|White Plumage': shinyById(10262),

  'Tatsugiri|Droopy Form': shinyById(10258),
  'Tatsugiri|Stretchy Form': shinyById(10259),

  'Dudunsparce|Three-Segment Form': shinyById(10255),

  'Poltchageist|Artisan Form': shinyForm(1012, 'artisan'),
  'Sinistcha|Masterpiece Form': shinyForm(1013, 'masterpiece'),

  // Special gender sprites
  'Oinkologne||Female': shinyById(10254),
  'Basculegion||Female': shinyById(10248),
  'Sneasel|Hisuian|Female':
    `${spriteBase}/female/10235.png`
}

// Unown
const unownForms = {
  B: 'b',
  C: 'c',
  D: 'd',
  E: 'e',
  F: 'f',
  G: 'g',
  H: 'h',
  I: 'i',
  J: 'j',
  K: 'k',
  L: 'l',
  M: 'm',
  N: 'n',
  O: 'o',
  P: 'p',
  Q: 'q',
  R: 'r',
  S: 's',
  T: 't',
  U: 'u',
  V: 'v',
  W: 'w',
  X: 'x',
  Y: 'y',
  Z: 'z',
  '!': 'exclamation',
  '?': 'question'
}

for (const [formName, spriteName] of Object.entries(unownForms)) {
  formSprites[`Unown|${formName}`] =
    shinyForm(201, spriteName)
}

// Deerling / Sawsbuck
for (const season of ['Summer', 'Autumn', 'Winter']) {
  const spriteName = season.toLowerCase()

  formSprites[`Deerling|${season} Form`] =
    shinyForm(585, spriteName)

  formSprites[`Sawsbuck|${season} Form`] =
    shinyForm(586, spriteName)
}

// Vivillon
const vivillonPatterns = {
  'Icy Snow Pattern': 'icy-snow',
  'Polar Pattern': 'polar',
  'Tundra Pattern': 'tundra',
  'Continental Pattern': 'continental',
  'Garden Pattern': 'garden',
  'Elegant Pattern': 'elegant',
  'Modern Pattern': 'modern',
  'Marine Pattern': 'marine',
  'Archipelago Pattern': 'archipelago',
  'High Plains Pattern': 'high-plains',
  'Sandstorm Pattern': 'sandstorm',
  'River Pattern': 'river',
  'Monsoon Pattern': 'monsoon',
  'Savanna Pattern': 'savanna',
  'Sun Pattern': 'sun',
  'Ocean Pattern': 'ocean',
  'Jungle Pattern': 'jungle',
  'Fancy Pattern': 'fancy'
}

for (const [formName, spriteName] of Object.entries(vivillonPatterns)) {
  formSprites[`Vivillon|${formName}`] =
    shinyForm(666, spriteName)
}

// Flabébé / Floette / Florges
const flowerColours = {
  'Yellow Flower': 'yellow',
  'Orange Flower': 'orange',
  'Blue Flower': 'blue',
  'White Flower': 'white'
}

for (const [formName, spriteName] of Object.entries(flowerColours)) {
  formSprites[`Flabébé|${formName}`] =
    shinyForm(669, spriteName)

  formSprites[`Floette|${formName}`] =
    shinyForm(670, spriteName)

  formSprites[`Florges|${formName}`] =
    shinyForm(671, spriteName)
}

// Furfrou
const furfrouTrims = {
  'Heart Trim': '/furfrou/heart.png',
  'Star Trim': '/furfrou/star.png',
  'Diamond Trim': '/furfrou/diamond.png',
  'Debutante Trim': '/furfrou/debutante.png',
  'Matron Trim': '/furfrou/matron.png',
  'Dandy Trim': '/furfrou/dandy.png',
  'La Reine Trim': '/furfrou/la-reine.png',
  'Kabuki Trim': '/furfrou/kabuki.png',
  'Pharaoh Trim': '/furfrou/pharaoh.png'
}

for (const [formName, spritePath] of Object.entries(furfrouTrims)) {
  formSprites[`Furfrou|${formName}`] = spritePath
}

// Alcremie
const alcremieCreams = {
  'Vanilla Cream': 'vanilla-cream',
  'Ruby Cream': 'ruby-cream',
  'Matcha Cream': 'matcha-cream',
  'Mint Cream': 'mint-cream',
  'Lemon Cream': 'lemon-cream',
  'Salted Cream': 'salted-cream',
  'Ruby Swirl': 'ruby-swirl',
  'Caramel Swirl': 'caramel-swirl',
  'Rainbow Swirl': 'rainbow-swirl'
}

const alcremieSweets = {
  'Strawberry Sweet': 'strawberry-sweet',
  'Berry Sweet': 'berry-sweet',
  'Love Sweet': 'love-sweet',
  'Star Sweet': 'star-sweet',
  'Clover Sweet': 'clover-sweet',
  'Flower Sweet': 'flower-sweet',
  'Ribbon Sweet': 'ribbon-sweet'
}

for (const [creamName, creamSlug] of Object.entries(alcremieCreams)) {
  for (const [sweetName, sweetSlug] of Object.entries(alcremieSweets)) {
    const formName = `${creamName} - ${sweetName}`

    // Vanilla Cream + Strawberry Sweet is Alcremie's default sprite
    if (
      creamName === 'Vanilla Cream' &&
      sweetName === 'Strawberry Sweet'
    ) {
      continue
    }

    formSprites[`Alcremie|${formName}`] =
      shinyForm(
        869,
        `${creamSlug}-${sweetSlug}`
      )
  }
}