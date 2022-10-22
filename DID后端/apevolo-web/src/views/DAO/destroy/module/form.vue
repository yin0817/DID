<template>
  <el-dialog
    append-to-body
    :title="isEdit ? '编辑' : '添加'"
    :visible.sync='show'
    :close-on-click-modal="false"
    :before-close='reset'
    width="500px"
  >
    <el-form
      ref="form"
      :model="form.data"
      :rules="form.rules"
      size="small"
      label-width="80px"
    >
      <el-form-item
        label="销毁原因"
        prop="memo"
      >
        <el-input
          v-model="form.data.memo"
          placeholder='请输入销毁原因'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="销毁地址"
        prop="hashCode"
      >
        <el-input
          v-model="form.data.hashCode"
          placeholder='请输入hash地址'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="销毁数量"
        prop="eotc"
      >
        <el-input-number
          style='width: 100%;'
          v-model="form.data.eotc"
          placeholder='请输入eotc数量'
          :precision="6"
          :step="0.000001"
        ></el-input-number>
      </el-form-item>
      <el-form-item
        label="销毁日期"
        prop="destructionDate"
      >
        <el-date-picker
          v-model="form.data.destructionDate"
          style='width: 100%;'
          type="datetime"
          placeholder="请选择销毁日期"
          align="right">
        </el-date-picker>
      </el-form-item>
    </el-form>
    <div
      slot="footer"
      class="dialog-footer"
    >
      <el-button
        type="text"
        @click="toggle(false)"
      >
        取消
      </el-button>
      <el-button
        :loading="form.loading"
        :disabled="form.loading"
        type="primary"
        @click="submit"
      >
        确认
      </el-button>
    </div>
  </el-dialog>
</template>

<script>
import { add } from '@/api/DAO/destroy'
export default {
  data() {
    return {
      show: false,
      isEdit: false,
      title: '',
      form: {
        loading: false,
        data: {
          destructionId: null,
          hashCode: '',
          eotc: '',
          memo: '',
          destructionDate: '',
          remark: ''
        },
        rules: {
          hashCode: [{ required: true, message: '请输入hash', trigger: 'blur' }],
          eotc: [{ required: true, message: '请输入eotc数量', trigger: 'blur' }],
          memo: [{ required: true, message: '请输入memo', trigger: 'blur' }],
          destructionDate: [{ required: true, message: '请选择销毁日期时间', trigger: 'blur' }]
        }
      }
    }
  },
  methods: {
    toggle(show, data) {
      this.show = show
      this.isEdit = !!data
      !!data && this.$nextTick().then(() => {
        Object.assign(this.form.data, data)
      })
      !show && this.reset()
    },
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.form.data.destructionId = null
      this.isEdit = false
      done && done()
    },
    submit() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.form.loading = true
          add(this.form.data).then(() => {
            this.$message.success(`${this.isEdit ? '编辑' : '添加'}成功！`)
            this.toggle(false)
            this.$emit('refresh')
          }).catch(() => {
            this.$message.error(`${this.isEdit ? '编辑' : '添加'}失败！`)
          }).finally(() => {
            this.form.loading = false
          })
        }
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
