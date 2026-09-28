<script setup>
import { ref } from 'vue'

const message = ref('')
const importing = ref(false)

async function exportCollection() {
  try {
    const response = await fetch('http://localhost:5119/api/shinies/export')

    if (!response.ok) {
      throw new Error('Export failed')
    }

    const backup = await response.json()

    const blob = new Blob(
      [JSON.stringify(backup, null, 2)],
      { type: 'application/json' }
    )

    const url = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = url
    link.download = `ultimate-shiny-dex-backup-${new Date()
      .toISOString()
      .slice(0, 10)}.json`

    link.click()

    URL.revokeObjectURL(url)

    message.value = 'Collection exported successfully!'
  } catch (error) {
    console.error(error)
    message.value = 'Something went wrong while exporting.'
  }
}

async function importCollection(event) {
  const file = event.target.files[0]

  if (!file) {
    return
  }

  const confirmed = confirm(
    'Importing will replace your current collection. Continue?'
  )

  if (!confirmed) {
    event.target.value = ''
    return
  }

  importing.value = true
  message.value = ''

  try {
    const fileText = await file.text()
    const backup = JSON.parse(fileText)

    const response = await fetch(
      'http://localhost:5119/api/shinies/import',
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(backup)
      }
    )

    if (!response.ok) {
      throw new Error('Import failed')
    }

    const result = await response.json()

    message.value =
      `Imported ${result.imported} shinies successfully!`
  } catch (error) {
    console.error(error)
    message.value = 'Something went wrong while importing.'
  } finally {
    importing.value = false
    event.target.value = ''
  }
}
</script>

<template>
  <main class="backup-page">
    <section class="backup-card">
      <h1>Collection Backup</h1>

      <p class="description">
        Export your shiny collection to a JSON file or import a previous backup.
      </p>

      <p class="warning">
        Custom uploaded images are not included in backups.
      </p>

      <div class="backup-section">
        <h2>Export Collection</h2>

        <p>
          Download your current shiny collection as a backup file.
        </p>

        <button @click="exportCollection">
          Export Collection
        </button>
      </div>

      <div class="backup-section">
        <h2>Import Collection</h2>

        <p>
          Importing a backup will replace the collection currently stored on this device.
        </p>

        <label class="import-button">
          {{ importing ? 'Importing...' : 'Choose Backup File' }}

          <input
            type="file"
            accept=".json,application/json"
            :disabled="importing"
            @change="importCollection"
          />
        </label>
      </div>

      <p v-if="message" class="message">
        {{ message }}
      </p>
    </section>
  </main>
</template>

<style scoped>
.backup-page {
  display: flex;
  justify-content: center;
  padding: 40px 20px;
}

.backup-card {
  width: 100%;
  max-width: 650px;
  padding: 30px;
  border: 1px solid #ddd;
  border-radius: 20px;
  background: white;
}

h1 {
  margin-top: 0;
}

.description {
  color: #555;
}

.warning {
  padding: 12px;
  border-radius: 10px;
  background: #f3f4f6;
}

.backup-section {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #ddd;
}

button,
.import-button {
  display: inline-block;
  margin-top: 10px;
  padding: 10px 18px;
  border: none;
  border-radius: 10px;
  background: #111827;
  color: white;
  font-weight: 600;
  cursor: pointer;
}

.import-button input {
  display: none;
}

button:hover,
.import-button:hover {
  opacity: 0.9;
}

.message {
  margin-top: 25px;
  font-weight: 600;
}
</style>