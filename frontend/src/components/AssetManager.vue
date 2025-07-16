<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { api } from '@/services/api';

// ───────────── 데이터 모델 ─────────────
interface Asset {
  id: number;
  name: string;
  ip_address: string;
  port: number;
  protocol: string;
  description: string | null;
  ssh_user: string; // SSH 사용자명
}

// ───────────── 상태 ─────────────
const assets = ref<Asset[]>([]);
const form = ref({
  name: '',
  ip: '',
  port: 22,
  protocol: 'tcp',
  description: '',
});
const toast = ref('');                 // 간단 토스트 메시지

// ───────────── API 호출 ─────────────
const load = async () => {
  assets.value = (await api.get('/assets')).data;
};
const reset = () =>
  Object.assign(form.value, {
    name: '',
    ip: '',
    port: 22,
    protocol: 'tcp',
    description: '',
  });

async function add() {
  await api.post('/assets', {
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

// ───────────── SSH 명령 복사 ─────────────
async function copySSH(asset: Asset, os: 'win' | 'mac') {
  const user = asset.name;             // 필요하면 form에서 user 입력받도록 수정
  const cmd =
    os === 'win'
      ? `ssh ${asset.ssh_user}@${asset.ip_address}`
      : `ssh ${asset.ssh_user}@${asset.ip_address}`;

  try {
    await navigator.clipboard.writeText(cmd);
    showToast(`복사 완료: ${cmd}`);
  } catch {
    showToast('클립보드 접근이 차단되었습니다');
  }
}

function showToast(msg: string) {
  toast.value = msg;
  setTimeout(() => (toast.value = ''), 2500);
}

onMounted(load);
</script>

<template>
  <div v-if="toast" class="fixed bottom-4 right-4 bg-black/80 text-white px-3 py-2 rounded">
    {{ toast }}
  </div>

  <h2 class="text-xl font-semibold mb-2">자산 목록</h2>
  <table class="border mb-4 w-full text-sm">
    <thead class="bg-gray-100">
      <tr>
        <th>ID</th>
        <th>이름</th>
        <th>IP</th>
        <th>Port</th>
        <th colspan="3">Action</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="a in assets" :key="a.id" class="odd:bg-gray-50">
        <td class="px-2">{{ a.id }}</td>
        <td class="px-2">{{ a.name }}</td>
        <td class="px-2">{{ a.ip_address }}</td>
        <td class="px-2">{{ a.port }}</td>

        <!-- 복사 버튼들 -->
        <td>
          <button class="text-blue-600 underline" @click="copySSH(a, 'win')">
            Win SSH 복사
          </button>
        </td>
        <td>
          <button class="text-green-600 underline" @click="copySSH(a, 'mac')">
            Mac SSH 복사
          </button>
        </td>

        <!-- 삭제 -->
        <td>
          <button class="text-red-600 underline" @click="remove(a.id)">
            삭제
          </button>
        </td>
      </tr>
    </tbody>
  </table>

  <h2 class="text-xl font-semibold mb-2">신규 등록</h2>
  <form @submit.prevent="add" class="flex flex-wrap gap-2 items-end">
    <input v-model="form.name" placeholder="이름(계정)" class="border px-2 py-1" required />

    <input v-model="form.ip" placeholder="IP" class="border px-2 py-1" required />

    <input
      v-model.number="form.port"
      type="number"
      min="1"
      max="65535"
      class="border w-24 px-2 py-1"
      title="SSH 포트"
    />

    <select v-model="form.protocol" class="border px-2 py-1">
      <option>tcp</option>
      <option>udp</option>
    </select>

    <input v-model="form.description" placeholder="비고" class="border flex-1 px-2 py-1" />

    <button type="submit" class="bg-blue-600 text-white px-3 py-1 rounded">
      추가
    </button>
  </form>
</template>

<style scoped>
table th,
table td {
  border: 1px solid #ddd;
  padding: 4px 6px;
}
</style>
