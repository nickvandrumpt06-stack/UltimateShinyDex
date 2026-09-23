<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { shinies } from '../data/shinies'

const router = useRouter()

const pokemon = ref('')
const nickname = ref('')
const nature = ref('')
const game = ref('')
const ball = ref('')

function addShiny() {
    shinies.value.push({
        id: shinies.value.length + 1,
        pokemon: pokemon.value,
        nickname: nickname.value,
        nature: nature.value,
        game: game.value,
        ball: ball.value
    })
    router.push('/')
}
</script>

<template>
    <main>
        <h1>Add a Shiny</h1>

        <form @submit.prevent="addShiny">
            <div>
                <label for="pokemon">Pokemon:</label>
                <input type="text" id="pokemon" v-model="pokemon" />
            </div>

            <div>
                <label for="nickname">Nickname:</label>
                <input type="text" id="nickname" v-model="nickname" />
            </div>

            <div>
                <label for="nature">Nature:</label>
                <input type="text" id="nature" v-model="nature" />
            </div>

            <div>
                <label for="game">Game:</label>
                <input type="text" id="game" v-model="game" />
            </div>

            <div>
                <label for="ball">Ball:</label>
                <input type="text" id="ball" v-model="ball" />
            </div>

            <button type="submit">Add Shiny</button>
        </form>
    </main>
</template>
