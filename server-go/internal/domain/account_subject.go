package domain

type AccountSubject struct {
	ID         int64  `gorm:"primaryKey;column:_id" json:"id"`
	No         string `gorm:"column:_no" json:"no"`
	Name       string `gorm:"column:_name" json:"name"`
	FullName   string `gorm:"column:_fullName" json:"fullName"`
	GroupID    int64  `gorm:"column:_groupId" json:"groupId"`
	ParentID   int64  `gorm:"column:_parentId" json:"parentId"`
	RootID     int64  `gorm:"column:_rootId" json:"rootId"`
	Level      int    `gorm:"column:_level" json:"level"`
	IsHasChild bool   `gorm:"column:_isHasChild" json:"isHasChild"`
	Direction  int    `gorm:"column:_direction" json:"direction"`
	IsDeleted  int    `gorm:"column:_isDeleted" json:"isDeleted"`
}

func (AccountSubject) TableName() string {
	return "_accountsubject"
}
