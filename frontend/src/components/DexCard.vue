<script setup>
import { ref, computed } from 'vue'
import alphaSymbol from '../assets/alpha-symbol.png'
import { ballSprites } from '../data/ballSprites'
import { marks } from '../data/marks'
import { RouterLink } from 'vue-router'

const expanded = ref(false)

const props = defineProps({
  entry: {
    type: Object,
    required: true
  }
})

const currentIndex = ref(0)

const currentShiny = computed(() => {
  return props.entry.shinies[currentIndex.value]
})

const spriteUrl = computed(() => {
  if (currentShiny.value?.customSpritePath) {
    return `http://localhost:5119${currentShiny.value.customSpritePath}`
  }

  if (!props.entry.dexNumber) {
    return null
  }

  return `https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/${props.entry.dexNumber}.png`
})

const ballImageUrl = computed(() => {
  return ballSprites[currentShiny.value?.ball] ?? null
})

const currentMarkImages = computed(() => {
  if (!currentShiny.value?.marks) {
    return []
  }

  return currentShiny.value.marks.map((shinyMark) => {
    const markData = marks.find(
      (mark) => mark.name === shinyMark.markName
    )

    return {
      name: shinyMark.markName,
      image: markData?.image ?? null
    }
  })
})

function previousShiny() {
  if (currentIndex.value > 0) {
    currentIndex.value--
  }
}

function nextShiny() {
  if (currentIndex.value < props.entry.shinies.length - 1) {
    currentIndex.value++
  }
}

function toggleExpanded() {
  if (props.entry.obtained) {
    expanded.value = !expanded.value
  }
}
</script>

<template>
  <article
    class="dex-card"
    :class="{ clickable: entry.obtained }"
    @click="toggleExpanded"
  >
    <div class="sprite-placeholder">
      <img
        v-if="entry.obtained"
        :src="spriteUrl"
        :alt="`Shiny ${entry.name}`"
        class="pokemon-sprite"

        
      />

      <span v-else>
        {{ entry.name }}
      </span>
    </div>

    <h2>
      #{{ String(entry.dexNumber).padStart(3, '0') }}
      {{ entry.name }}
    </h2>

    <p v-if="entry.form" class="form">
      {{ entry.form }}
    </p>

    <p v-if="entry.gender" class="gender">
      {{ entry.gender }}
    </p>

    <div v-if="entry.obtained">
      <p
        v-if="currentShiny.nickname"
        class="nickname"
      >
        "{{ currentShiny.nickname }}"
      </p>

      <div class="summary-icons">
  <img
    v-if="ballImageUrl"
    :src="ballImageUrl"
    :alt="currentShiny.ball"
    :title="currentShiny.ball"
    class="ball-icon"
  />

  <img
    v-if="currentShiny.isAlpha"
    :src="alphaSymbol"
    alt="Alpha Pokémon"
    title="Alpha Pokémon"
    class="alpha-icon"
  />

  <img
  v-for="mark in currentMarkImages"
  :key="mark.name"
  v-show="mark.image"
  :src="mark.image"
  :alt="mark.name"
  :title="mark.name"
  class="mark-icon"
/>
</div>

      <div
        v-if="expanded"
        class="owned-info"
      >
        <p>
          <strong>Nature:</strong>
          {{ currentShiny.nature }}
        </p>

        <p>
          <strong>Game:</strong>
          {{ currentShiny.game }}
        </p>

        <p>
          <strong>Ball:</strong>
          {{ currentShiny.ball }}
        </p>

        <p>
          <strong>Method:</strong>
          {{ currentShiny.method }}
        </p>

        <p v-if="currentShiny.encounters !== null">
          <strong>Encounters:</strong>
          {{ currentShiny.encounters }}
        </p>

        <p v-if="currentShiny.isAlpha">
          <strong>Alpha:</strong>
          Yes
        </p>

        <div
  v-if="currentShiny.marks?.length"
  class="mark-details"
>
  <strong>Marks:</strong>

  <ul>
    <li
      v-for="mark in currentShiny.marks"
      :key="mark.id"
    >
      {{ mark.markName }}
    </li>
  </ul>
</div>
      </div>

      <p class="details-hint">
        {{ expanded ? 'Click to hide details' : 'Click to view details' }}
      </p>

      <RouterLink
  class="edit-link"
  :to="`/edit/${currentShiny.id}`"
  @click.stop
>
  Edit
</RouterLink>
    </div>

    <div v-else class="missing">
      <p>Not obtained</p>
    </div>

    <div
      v-if="entry.shinies.length > 1"
      class="duplicate-navigation"
    >
      <button
        @click.stop="previousShiny"
        :disabled="currentIndex === 0"
      >
        ←
      </button>

      <span>
        {{ currentIndex + 1 }} / {{ entry.shinies.length }}
      </span>

      <button
        @click.stop="nextShiny"
        :disabled="currentIndex === entry.shinies.length - 1"
      >
        →
      </button>
    </div>
  </article>
</template>

<style scoped>
.dex-card {
  padding: 20px;
  border-radius: 18px;
  background: white;
  border: 1px solid #e5e7eb;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.08);
  text-align: center;
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.dex-card.clickable {
  cursor: pointer;
}

.dex-card.clickable:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.12);
}

.sprite-placeholder {
  height: 150px;
  border-radius: 14px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
}

.pokemon-sprite {
  width: 140px;
  height: 140px;
  object-fit: contain;
  image-rendering: pixelated;
}

.form,
.gender {
  margin: 4px 0;
  color: #6b7280;
  font-weight: 600;
}

.nickname {
  margin: 8px 0;
  color: #6b7280;
  font-style: italic;
  font-weight: 600;
}

.missing {
  opacity: 0.45;
}

.owned-info {
  margin-top: 14px;
  text-align: left;
}

.owned-info p {
  margin: 6px 0;
}

.details-hint {
  margin-top: 12px;
  font-size: 0.8rem;
  color: #9ca3af;
}

.duplicate-navigation {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin-top: 16px;
}

.duplicate-navigation button {
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  font-size: 1.1rem;
}

.duplicate-navigation button:disabled {
  opacity: 0.3;
  cursor: default;
}

.duplicate-navigation span {
  font-weight: 600;
}

.summary-icons {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin-top: 10px;
}

.ball-icon {
  width: 32px;
  height: 32px;
  object-fit: contain;
  image-rendering: pixelated;
}

.alpha-icon {
  width: 32px;
  height: 32px;
  object-fit: contain;
}

.mark-icon {
  width: 32px;
  height: 32px;
  object-fit: contain;
}

.mark-details ul {
  margin: 6px 0 0;
  padding-left: 20px;
}

.edit-link {
  display: inline-block;
  margin-top: 10px;
  padding: 8px 14px;
  border-radius: 10px;
  background: #111827;
  color: white;
  text-decoration: none;
  font-size: 0.9rem;
  font-weight: 600;
}

.edit-link:hover {
  opacity: 0.9;
}
</style>