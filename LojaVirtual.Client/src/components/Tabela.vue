<script>
    export default{
        name : "Tabela",
        data(){
            return {
                produtos : []
            }
        },
        async created(){
            const url = "https://localhost:7209/api/v1/produtos"
            const resposta = await fetch(url)
            this.produtos = await resposta.json()
            //console.log(this.produtos)
        }
    }
</script>

<template>
    <table border>
        <thead>
            <th>Nome</th>
            <th>Descrição</th>
            <th>Preço</th>
        </thead>
        <tbody>
            <tr v-for="produto in produtos">
                <td>{{ produto.nome }}</td>
                <td>{{ produto.descricao }}</td>
                <td>{{ produto.valor }}</td>
            </tr>
        </tbody>
    </table>
</template>

<style scoped>
</style>
