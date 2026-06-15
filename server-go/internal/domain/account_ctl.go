package domain

type AccountCtl struct {
	ID      int64  `gorm:"primaryKey;column:_id" json:"id"`
	No      string `gorm:"column:_no" json:"no"`
	Name    string `gorm:"column:_name" json:"name"`
	ConnStr string `gorm:"column:_connstr" json:"connstr"`
}

func (AccountCtl) TableName() string {
	return "_AccountCtl"
}
