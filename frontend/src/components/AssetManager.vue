<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { api } from '@/services/api';

interface Asset {
  id: number;
  name: string;
  ip_address: string;
  port: number;
  protocol: string;
  description: string | null;
  ssh_user?: string;
}

const assets = ref<Asset[]>([]);
const lastInsertedId = ref<number | null>(null);

const form = reactive({
  name: '',
  ssh_user: '',
  ip: '',
  port: 22,
  protocol: 'tcp',
  description: '',
});

const search = ref('');
const sortKey = ref<'name' | 'ip_address' | 'port'>('name');
const sortAsc = ref(true);

const toast = ref('');

// ── computed
const filteredAssets = computed(() => {
  const q = search.value.trim().toLowerCase();
  let list = assets.value;
  if (q) {
    list = list.filter(a =>
      [a.name, a.ip_address, String(a.port)].some(v => v?.toLowerCase().includes(q))
    );
  }
  return [...list].sort((a, b) => {
    const ak = (a as any)[sortKey.value];
    const bk = (b as any)[sortKey.value];
    if (ak < bk) return sortAsc.value ? -1 : 1;
    if (ak > bk) return sortAsc.value ? 1 : -1;
    return 0;
  });
});

watch(toast, v => {
  if (!v) return;
  const t = setTimeout(() => (toast.value = ''), 2000);
  return () => clearTimeout(t);
});

// ── api
const load = async () => {
  assets.value = (await api.get('/assets')).data;
};
const reset = () =>
  Object.assign(form, {
    name: '',
    ssh_user: '',
    ip: '',
    port: 22,
    protocol: 'tcp',
    description: '',
  });

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
  fireToast('등록 완료!');
}

async function remove(id: number) {
  await api.delete(`/assets/${id}`);
  await load();
  fireToast('삭제 완료!');
}

// ── ssh
function sshCmd(a: Asset) {
  const user = a.ssh_user || a.name;
  return `ssh ${user}@${a.ip_address} -p ${a.port}`;
}

// ── copy directive
const vCopy = {
  async mounted(el: HTMLElement, binding: any) {
    el.addEventListener('click', async () => {
      try {
        await navigator.clipboard.writeText(binding.value);
        fireToast('복사 완료!');
      } catch {
        fireToast('클립보드 권한 오류');
      }
    });
  },
};

function fireToast(msg: string) {
  toast.value = msg;
}

function toggleSort(key: 'name' | 'ip_address' | 'port') {
  if (sortKey.value === key) sortAsc.value = !sortAsc.value;
  else {
    sortKey.value = key;
    sortAsc.value = true;
  }
}

onMounted(load);
</script>

<template>
  <!-- Toast -->
  <Teleport to="body">
    <transition name="fade">
      <div
        v-if="toast"
        class="toast"
      >
        {{ toast }}
      </div>
    </transition>
  </Teleport>

  <!-- Top bar -->
  <div class="topbar">
    <h2>자산 목록</h2>
    <div class="tools">
      <input v-model.trim="search" placeholder="검색 (이름/IP/포트)" />
    </div>
  </div>

  <!-- Table -->
  <div class="table-wrap">
    <table>
      <thead>
        <tr>
          <th @click="toggleSort('name')" :class="{ sort: sortKey==='name' }">
            이름 <span v-if="sortKey==='name'">{{ sortAsc ? '▲' : '▼' }}</span>
          </th>
          <th @click="toggleSort('ip_address')" :class="{ sort: sortKey==='ip_address' }">
            IP <span v-if="sortKey==='ip_address'">{{ sortAsc ? '▲' : '▼' }}</span>
          </th>
          <th @click="toggleSort('port')" :class="{ sort: sortKey==='port' }">
            Port <span v-if="sortKey==='port'">{{ sortAsc ? '▲' : '▼' }}</span>
          </th>
          <th class="action-col">SSH</th>
          <th class="action-col">삭제</th>
        </tr>
      </thead>

      <transition-group tag="tbody" name="list">
        <tr
          v-for="a in filteredAssets"
          :key="a.id"
          :class="[{ highlight: lastInsertedId === a.id }]"
        >
          <td>{{ a.name }}</td>
          <td class="mono">{{ a.ip_address }}</td>
          <td>{{ a.port }}</td>
          <td>
            <button v-copy="sshCmd(a)" class="btn small ghost">
              📋 복사
            </button>
          </td>
          <td>
            <button class="btn small danger" @click="remove(a.id)">
              삭제
            </button>
          </td>
        </tr>
      </transition-group>
    </table>
  </div>

  <!-- Form -->
  <h3 class="mt-24">신규 등록</h3>
  <form @submit.prevent="add" class="form">
    <input v-model.trim="form.name" placeholder="이름(계정)" required />
    <input v-model.trim="form.ssh_user" placeholder="SSH 사용자 (미입력 시 이름)" />
    <input v-model.trim="form.ip" placeholder="IP" required />
    <input v-model.number="form.port" type="number" min="1" max="65535" placeholder="Port" />
    <select v-model="form.protocol">
      <option>tcp</option>
      <option>udp</option>
    </select>
    <input v-model.trim="form.description" placeholder="비고" class="flex-1" />
    <button type="submit" class="btn primary">추가</button>
  </form>
</template>

<style scoped>
/* 기본 색 */
:root {
  --border: #ddd;
  --text: #333;
  --accent: #2563eb;
  --accent-light: #e0ecff;
  --danger: #dc2626;
  --danger-light: #fde2e2;
}

/* Toast */
.toast {
  position: fixed;
  bottom: 24px;
  right: 24px;
  background: rgba(0,0,0,.8);
  color: #fff;
  padding: 8px 14px;
  border-radius: 6px;
  font-size: 13px;
  box-shadow: 0 2px 8px rgba(0,0,0,.3);
  z-index: 9999;
}

/* top bar */
.topbar{
  display:flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom:16px;
}
.topbar h2{
  margin:0;
  font-size:20px;
  font-weight:600;
}
.topbar .tools{
  display:flex;
  gap:8px;
}
.topbar input{
  border:1px solid var(--border);
  padding:6px 10px;
  border-radius:4px;
  font-size:13px;
  width:220px;
}

/* table */
.table-wrap{
  border:1px solid var(--border);
  border-radius:6px;
  overflow:auto;
  max-height: 360px;
}
table{
  border-collapse:collapse;
  width:100%;
  font-size:13px;
}
thead{
  position: sticky;
  top: 0;
  background:#f8f9fa;
  z-index:1;
}
th,td{
  padding:8px 10px;
  border-bottom:1px solid var(--border);
  text-align:left;
}
th.sort{
  color:var(--accent);
}
tbody tr:hover{
  background:#fafafa;
}
.mono{
  font-family: ui-monospace, SFMono-Regular, Menlo, monospace;
}
.highlight{
  animation: flash 1.6s ease-out;
}
@keyframes flash{
  0%{ background: #eaffea;}
  100%{ background: transparent;}
}
.action-col{
  width:90px;
}

/* form */
h3{
  font-size:18px;
  margin:24px 0 12px;
}
.form{
  display:flex;
  flex-wrap:wrap;
  gap:8px;
  font-size:13px;
}
.form input,
.form select{
  border:1px solid var(--border);
  padding:6px 10px;
  border-radius:4px;
  min-width:120px;
}

/* buttons */
.btn{
  border:1px solid var(--border);
  padding:6px 14px;
  border-radius:4px;
  font-size:13px;
  cursor:pointer;
  background:#fff;
  transition: background .12s;
}
.btn:hover{
  background:#fafafa;
}
.btn.small{
  padding:4px 8px;
  font-size:12px;
}
.btn.primary{
  background:var(--accent);
  border-color:var(--accent);
  color:#fff;
}
.btn.primary:hover{
  background:#1e4fc7;
}
.btn.ghost{
  background:var(--accent-light);
  border-color: var(--accent-light);
  color:var(--accent);
}
.btn.danger{
  background:var(--danger-light);
  border-color: var(--danger-light);
  color:var(--danger);
}
.btn.danger:hover{
  background:#fcd6d6;
}

/* transition */
.fade-enter-active,
.fade-leave-active{
  transition: opacity .15s;
}
.fade-enter-from,
.fade-leave-to{
  opacity:0;
}

.list-enter-active,
.list-leave-active{
  transition: all .15s;
}
.list-enter-from,
.list-leave-to{
  opacity:0;
  transform: translateY(-3px);
}
</style>
