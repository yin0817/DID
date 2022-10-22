<template>
  <div class="app-container">
    <!--  工具  -->
    <el-button size='small' type='primary' @click='edit(null)'>添加</el-button>
    <!--表格渲染-->
    <el-table
      ref="table"
      v-loading="table.loading"
      :data="table.data"
      row-key='noticeId'
      style="width: 100%"
    >
      <el-table-column prop="orderId" label="订单编号" />
      <el-table-column prop="orderType" label="订单类型" width='80px'>
        <template slot-scope='scope'>
          <el-tag v-if='scope.row.orderType === 0'>课程</el-tag>
          <el-tag type='success' v-else>系统</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="订单状态" width='270px'>
        <template slot-scope='scope'>
          <div v-if='scope.row.status === 0'>
            <el-tag>待支付</el-tag>
            <span class='time'>下单时间: {{transformDate(scope.row.createDate)}}</span>
          </div>
          <div v-else-if='scope.row.status === 1'>
            <el-tag type='success'>已支付</el-tag>
            <span class='time'>支付时间: {{transformDate(scope.row.createDate)}}</span>
          </div>
          <div v-else>
            <el-tag type='danger'>已取消</el-tag>
            <span class='time'>取消时间: {{transformDate(scope.row.createDate)}}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="quantity" label="订单数量" />
      <el-table-column prop="name" label="用户名称" />
      <el-table-column prop="phone" label="手机号码" />
      <el-table-column prop="wechat" label="微信号码" />
      <el-table-column prop="volunteer" label="志愿者" />
      <!--   编辑与删除   -->
      <el-table-column
        label="操作"
        width="180px"
        align="center"
        fixed="right"
      >
        <template slot-scope="scope">
          <el-button
            size='small'
            type='warning'
            @click='edit(scope.row)'
          >
            修改
          </el-button>
          <el-button
            size='small'
            type='danger'
            :loading='scope.row.loading'
            :disabled='scope.row.loading'
            @click='remove(scope.row)'
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <edit ref='edit' @refresh='getTable'></edit>
  </div>
</template>

<script>
import Edit from './module/form'
import { list, del } from '@/api/APP/order'
import { transformDate } from '@/utils'

export default {
  name: 'Order',
  components: {
    Edit
  },
  data() {
    return {
      table: {
        loading: false,
        query: {
          title: ''
        },
        data: []
      }
    }
  },
  created() {
    this.getTable()
  },
  methods: {
    transformDate,
    getTable() {
      this.table.loading = true
      list().then(res => {
        this.table.data = res.items.map(item => ({ ...item, loading: false }))
      }).finally(() => {
        this.table.loading = false
      })
    },
    edit(data) {
      let dom = this.$refs.edit
      dom.toggle(true, data)
      dom = null
    },
    remove(data) {
      this.$confirm('确定删除此记录').then(res => {
        data.loading = true
        del(data.orderId).then(res => {
          this.$message.success('删除成功！')
          this.getTable()
        }).catch(() => {
          this.$message.error('删除失败！')
        })
      }).catch(() => {
        console.log(2)
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.time {
  display: inline-block;
  margin-left: 10px;
}
</style>
