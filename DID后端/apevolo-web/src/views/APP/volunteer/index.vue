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
      <el-table-column prop="wechat" label="自愿者微信号" />
      <el-table-column prop="qrCode" label="自愿者微信二维码">
        <template slot-scope='scope'>
          <img width='100px' :src='transformSrc(scope.row.qrCode)' alt=''>
        </template>
      </el-table-column>
      <el-table-column prop="createDate" label="发布时间">
        <template slot-scope='scope'>
          {{ transformDate(scope.row.createDate) }}
        </template>
      </el-table-column>
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
import { list, del } from '@/api/APP/volunteer'
import { transformDate, transformSrc } from '@/utils'

export default {
  name: 'Volunteer',
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
    transformSrc,
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
        del(data.volunteerId).then(res => {
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
::v-deep .el-input-number .el-input__inner {
  text-align: left;
}
</style>
