package repository

import (
	"context"
	"finance-server/internal/domain"
)

type userRepository struct {
	mgr *SQLiteManager
}

func NewUserRepository(mgr *SQLiteManager) domain.UserRepository {
	return &userRepository{mgr: mgr}
}

func (r *userRepository) GetByUserName(ctx context.Context, tid int64, userName string) (*domain.User, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}
	var user domain.User
	err = db.WithContext(ctx).Where("_userName = ? AND _isDeleted = 0", userName).First(&user).Error
	return &user, err
}

func (r *userRepository) GetByID(ctx context.Context, tid int64, id int64) (*domain.User, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}
	var user domain.User
	err = db.WithContext(ctx).First(&user, "_userId = ?", id).Error
	return &user, err
}

func (r *userRepository) Create(ctx context.Context, tid int64, user *domain.User) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}
	return db.WithContext(ctx).Create(user).Error
}

func (r *userRepository) Update(ctx context.Context, tid int64, user *domain.User) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}
	return db.WithContext(ctx).Save(user).Error
}

func (r *userRepository) List(ctx context.Context, tid int64) ([]*domain.User, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}
	var users []*domain.User
	err = db.WithContext(ctx).Find(&users).Error
	return users, err
}
