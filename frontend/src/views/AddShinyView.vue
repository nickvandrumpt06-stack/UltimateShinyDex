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
                <input type="text" id="pokemon" v-model="pokemon" required />
            </div>

            <div>
                <label for="nickname">Nickname:</label>
                <input type="text" id="nickname" v-model="nickname" />
            </div>

            <div>
                <label for="nature">Nature:</label>
                <select id="nature" v-model="nature" required>
                    <option disabled value="">Select a nature</option>
                    <option value="Adamant">Adamant</option>
                    <option value="Bashful">Bashful</option>
                    <option value="Bold">Bold</option>
                    <option value="Brave">Brave</option>
                    <option value="Calm">Calm</option>
                    <option value="Careful">Careful</option>
                    <option value="Docile">Docile</option>
                    <option value="Gentle">Gentle</option>
                    <option value="Hardy">Hardy</option>
                    <option value="Hasty">Hasty</option>
                    <option value="Impish">Impish</option>
                    <option value="Jolly">Jolly</option>
                    <option value="Lax">Lax</option>
                    <option value="Lonely">Lonely</option>
                    <option value="Mild">Mild</option>
                    <option value="Modest">Modest</option>
                    <option value="Naive">Naive</option>
                    <option value="Naughty">Naughty</option>
                    <option value="Quiet">Quiet</option>
                    <option value="Quirky">Quirky</option>
                    <option value="Rash">Rash</option>
                    <option value="Relaxed">Relaxed</option>
                    <option value="Sassy">Sassy</option>
                    <option value="Serious">Serious</option>
                    <option value="Timid">Timid</option>
                </select>
            </div>

            <div>
                <label for="game">Game:</label>

                <select id="game" v-model="game" required>
                    <option disabled value="">Select a game</option>
                    <option value="Red">Red</option>
                    <option value="Blue">Blue</option>
                    <option value="Yellow">Yellow</option>
                    <option value="Gold">Gold</option>
                    <option value="Silver">Silver</option>
                    <option value="Crystal">Crystal</option>
                    <option value="Ruby">Ruby</option>
                    <option value="Sapphire">Sapphire</option>
                    <option value="Emerald">Emerald</option>
                    <option value="FireRed">FireRed</option>
                    <option value="LeafGreen">LeafGreen</option>
                    <option value="Diamond">Diamond</option>
                    <option value="Pearl">Pearl</option>
                    <option value="Platinum">Platinum</option>
                    <option value="HeartGold">HeartGold</option>
                    <option value="SoulSilver">SoulSilver</option>
                    <option value="Black">Black</option>
                    <option value="White">White</option>
                    <option value="Black2">Black 2</option>
                    <option value="White2">White 2</option>
                    <option value="X">X</option>
                    <option value="Y">Y</option>
                    <option value="Omega Ruby">Omega Ruby</option>
                    <option value="Alpha Sapphire">Alpha Sapphire</option>
                </select>
            </div>

            <div>
                <label for="ball">Ball:</label>

                <select id="ball" v-model="ball" required>
                    <option disabled value="">Select a ball</option>
                    <option value="Poké Ball">Poké Ball</option>
                    <option value="Great Ball">Great Ball</option>
                    <option value="Ultra Ball">Ultra Ball</option>
                    <option value="Master Ball">Master Ball</option>
                    <option value="Safari Ball">Safari Ball</option>
                    <option value="Net Ball">Net Ball</option>
                    <option value="Dive Ball">Dive Ball</option>
                    <option value="Nest Ball">Nest Ball</option>
                    <option value="Repeat Ball">Repeat Ball</option>
                    <option value="Timer Ball">Timer Ball</option>
                    <option value="Luxury Ball">Luxury Ball</option>
                    <option value="Premier Ball">Premier Ball</option>
                </select>
            </div>

            <button type="submit">Add Shiny</button>
        </form>
    </main>
</template>
