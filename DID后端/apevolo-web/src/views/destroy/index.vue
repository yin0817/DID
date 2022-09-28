<template>
  <div class="app-container">
    <!--  工具  -->
    <el-button size='small' type='primary' @click='edit(null)'>添加</el-button>
    <!--表格渲染-->
    <el-table
      ref="table"
      v-loading="table.loading"
      :data="table.data"
      row-key='destructionId'
      style="width: 100%"
    >
      <el-table-column prop="hashCode" label="销毁hash" />
      <el-table-column prop="eotc" label="eotc" />
      <el-table-column prop="memo" label="注释" />
      <el-table-column prop="remark" label="备注" />
      <el-table-column prop="createDate" label="创建日期">
        <template slot-scope='scope'>
          {{ transformUTCDate(scope.row.createDate) }}
        </template>
      </el-table-column>
      <el-table-column prop="destructionDate" label="销毁日期">
        <template slot-scope='scope'>
          {{ transformUTCDate(scope.row.destructionDate) }}
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
import { list, del } from '@/api/destroy'
import { transformUTCDate } from '@/utils'

export default {
  name: 'Destroy',
  components: {
    Edit
  },
  data() {
    return {
      table: {
        loading: false,
        data: []
      }
    }
  },
  created() {
    this.getTable()
  },
  methods: {
    transformUTCDate,
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
      dom.toggle(true)
      !!data && dom.init({
        destructionId: data.destructionId,
        hashCode: data.hashCode,
        eotc: data.eotc,
        memo: data.memo,
        destructionDate: data.destructionDate,
        remark: data.remark
      })
      dom = null
    },
    remove(data) {
      this.$confirm('确定删除此记录').then(res => {
        data.loading = true
        del(data.destructionId).then(res => {
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
