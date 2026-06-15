package domain

type User struct {
	UserID    int64  `gorm:"primaryKey;column:_userId" json:"userId"`
	UserName  string `gorm:"column:_userName" json:"userName"`
	PassWord  string `gorm:"column:_passWord" json:"password"`
	IsDeleted int    `gorm:"column:_isDeleted" json:"isDeleted"`
}

func (User) TableName() string {
	return "_userInfo"
}
