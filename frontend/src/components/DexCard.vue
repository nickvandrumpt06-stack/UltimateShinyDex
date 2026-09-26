<script setup>
defineProps({
  entry: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <article class="dex-card">
    <div class="sprite-placeholder">
      <span>
        {{ entry.name }}
      </span>
    </div>

    <h2>
      #{{ String(entry.dexNumber).padStart(3, '0') }}
      {{ entry.name }}
    </h2>

    <p v-if="entry.gender">
      {{ entry.gender }}
    </p>

    <div v-if="entry.obtained" class="owned-info">
  <p v-if="entry.shinies[0].nickname">
    <strong>Nickname:</strong>
    {{ entry.shinies[0].nickname }}
  </p>

  <p>
    <strong>Nature:</strong>
    {{ entry.shinies[0].nature }}
  </p>

  <p>
    <strong>Game:</strong>
    {{ entry.shinies[0].game }}
  </p>

  <p>
    <strong>Ball:</strong>
    {{ entry.shinies[0].ball }}
  </p>

  <p>
    <strong>Method:</strong>
    {{ entry.shinies[0].method }}
  </p>

  <p v-if="entry.shinies[0].encounters !== null">
    <strong>Encounters:</strong>
    {{ entry.shinies[0].encounters }}
  </p>

  <p v-if="entry.shinies[0].isAlpha">
    <strong>Alpha:</strong>
    Yes
  </p>

  <p v-if="entry.shinies[0].mark">
    <strong>Mark:</strong>
    {{ entry.shinies[0].mark }}
  </p>

  <p class="copies">
    Copies: {{ entry.shinies.length }}
  </p>
</div>

    <div v-else class="missing">
      <p>Not obtained</p>
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

.missing {
  opacity: 0.45;
}

.owned-info {
  margin-top: 12px;
  text-align: left;
}

.owned-info p {
  margin: 6px 0;
}

.copies {
  margin-top: 14px !important;
  text-align: center;
  font-weight: 600;
}
</style>