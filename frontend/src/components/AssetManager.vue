<script setup lang="ts">
import { ref, onMounted } from "vue";
import { api } from "@/services/api";

interface Asset {
  id: number;
  name: string;
  ip_address: string;
  port: number;
  protocol: string;
  description: string | null;
}

const assets = ref<Asset[]>([]);
const form = ref({ name: "", ip: "", port: 80, protocol: "tcp", description: "" });

const load = async () => (assets.value = (await api.get("/assets")).data);
const reset = () => (form.value = { name: "", ip: "", port: 80, protocol: "tcp", description: "" });

async function add() {
  await api.post("/assets", {
    name: form.value.name,
    ip_address: form.value.ip,
    port: form.value.port,
    protocol: form.value.protocol,
    description: form.value.description || null,
  });
  await load();
  reset();
}

async function remove(id: number) {
  await api.delete(`/assets/${id}`);
  await load();
}

onMounted(load);
</script>

<template>
  <h2 class="text-xl font-semibold mb-2">자산 목록</h2>
  <table class="border mb-4">
    <thead><tr><th>ID</th><th>이름</th><th>IP</th><th>Port</th><th></th></tr></thead>
    <tbody>
      <tr v-for="a in assets" :key="a.id">
        <td>{{ a.id }}</td><td>{{ a.name }}</td><td>{{ a.ip_address }}</td>
        <td>{{ a.port }}</td>
        <td><button @click="remove(a.id)">삭제</button></td>
      </tr>
    </tbody>
  </table>

  <h2 class="text-xl font-semibold mb-2">신규 등록</h2>
  <form @submit.prevent="add" class="flex gap-2">
    <input v-model="form.name" placeholder="이름" required />
    <input v-model="form.ip" placeholder="IP" required />
    <input v-model.number="form.port" type="number" min="1" max="65535" />
    <select v-model="form.protocol">
      <option>tcp</option><option>udp</option>
    </select>
    <input v-model="form.description" placeholder="비고" />
    <button type="submit">추가</button>
  </form>
</template>
