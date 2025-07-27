<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { api } from '@/services/api';

// ───────────── 모델 ─────────────
interface Asset {
  id: number;
  name: string;
  ip_address: string;
  port: number;
  protocol: string;
  description: string | null;
  ssh_user?: string;
}

// ───────────── 상태 ─────────────
const assets = ref<Asset[]>([]);
const lastInsertedId = ref<number | null>(null);

const form = reactive({
  name: '',
  ip: '',
  port: 22,
  protocol: 'tcp',
  description: '',
  ssh_user: '', // 필요 시 폼에 추가
});

const search = ref('');
const sortKey = ref<'name' | 'ip_address' | 'port'>('name');
const sortAsc = ref(true);

const toast = ref('');

// ───────────── 공통 기능 ─────────────
const filteredAssets = computed(() => {
  const q = search.value.trim().toLowerCase();
  let list = assets.value;
  if (q) {
    list = list.filter(
      a =>
        a.name.toLowerCase().includes(q) ||
        a.ip_address.toLowerCase().includes(q) ||
        String(a.port).includes(q)
    );
  }
  return [...list].sort((a, b) => {
    const ak = a[sortKey.value];
    const bk = b[sortKey.value];
    if (ak < bk) return sortAsc.value ? -1 : 1;
    if (ak > bk) return sortAsc.value ? 1 : -1;
    return 0;
  });
});

// 포트가 비어 있으면 프로토콜 tcp일 때 22 자동
watch(
  () => form.protocol,
  proto => {
    if (proto === 'tcp' && !form.port) form.port = 22;
  }
);

// 토스트 자동 숨김
watch(toast, v => {
  if (!v) return;
  const t = setTimeout(() => (toast.value = ''), 2500);
  return () => clearTimeout(t);
});

// ───────────── API ─────────────
const load = async () => {
  assets.value = (await api.get('/assets')).data;
};
const reset = () => {
  Object.assign(form, {
    name: '',
    ip: '',
    port: 22,
    protocol: 'tcp',
    description: '',
    ssh_user: '',
  });
};

async function add() {
  const res = await api.post('/assets', {
    name: form.name,
    ip_address: form.ip,
    port: form.port,
    protocol: form.protocol,
    description: form.description || null,
    ssh_user: form.ssh_user || form.name,
  });
  lastInsertedId.value = res.data.id;
  await load();
  reset();
  showToast('등록 완료!');
}

async function remove(id: number) {
  await api.delete(`/assets/${id}`);
  await load();
  showToast('삭제 완료!');
}

// ───────────── SSH ─────────────
function sshCmd(asset: Asset) {
  const user = asset.ssh_user || asset.name;
  return `ssh ${user}@${asset.ip_address} -p ${asset.port}`;
}

// ───────────── Clipboard (커스텀 디렉티브) ─────────────
const vCopy = {
  async mounted(el: HTMLElement, binding: any) {
    el.onclick = async () => {
      try {
        await navigator.clipboard.writeText(binding.value);
        showToast('복사 완료!');
      } catch {
        showToast('클립보드 권한 오류');
      }
    };
  },
};

// ───────────── 유틸 ─────────────
function showToast(msg: string) {
  toast.value = msg;
}

onMounted(load);
</script>

<template>
  <!-- Toast: Teleport + Transition -->
  <Teleport to="body">
    <Transition name="fade">
      <div v-if="toast" class="fixed bottom-4 right-4 bg-black/80 text-white px-3 py-2 rounded shadow">
        {{ toast }}
      </div>
    </Transition>
  </Teleport>

  <!-- 검색 / 정렬 -->
  <div class="flex gap-2 mb-3 items-center text-sm">
    <input v-model.trim="search" placeholder="검색 (이름/IP/포트)" class="border px-2 py-1 flex-1" />
    <select v-model="sortKey" class="border px-2 py-1">
      <option value="name">이름</option>
      <option value="ip_address">IP</option>
      <option value="port">Port</option>
    </select>
    <button class="border px-2 py-1" @click="sortAsc = !sortAsc">
      {{ sortAsc ? '▲' : '▼' }}
    </button>
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

    <TransitionGroup tag="tbody" name="list">
      <tr
        v-for="a in filteredAssets"
        :key="a.id"
        :class="[
          'odd:bg-gray-50',
          lastInsertedId === a.id ? 'bg-green-50 animate-pulse' : ''
        ]"
      >
        <td class="px-2">{{ a.id }}</td>
        <td class="px-2">{{ a.name }}</td>
        <td class="px-2">{{ a.ip_address }}</td>
        <td class="px-2">{{ a.port }}</td>

        <td>
          <button v-copy="sshCmd(a)" class="text-blue-600 underline">SSH 복사</button>
        </td>
        <td>
          <button class="text-gray-600 underline" @click="showToast(sshCmd(a))">명령 보기</button>
        </td>
        <td>
          <button class="text-red-600 underline" @click="remove(a.id)">삭제</button>
        </td>
      </tr>
    </TransitionGroup>
  </table>

  <h2 class="text-xl font-semibold mb-2">신규 등록</h2>
  <form @submit.prevent="add" class="flex flex-wrap gap-2 items-end text-sm">
    <input v-model.trim="form.name" placeholder="이름(계정)" class="border px-2 py-1" required />
    <input v-model.trim="form.ssh_user" placeholder="SSH 사용자 (미입력 시 이름 사용)" class="border px-2 py-1" />
    <input v-model.trim="form.ip" placeholder="IP" class="border px-2 py-1" required />

    <input
      v-model.number="form.port"
      type="number"
      min="1"
      max="65535"
      class="border w-20 px-2 py-1"
      title="SSH 포트"
    />

    <select v-model="form.protocol" class="border px-2 py-1">
      <option>tcp</option>
      <option>udp</option>
    </select>

    <input v-model.trim="form.description" placeholder="비고" class="border flex-1 px-2 py-1" />

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

/* Transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity .2s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.list-enter-active,
.list-leave-active {
  transition: all .2s;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
