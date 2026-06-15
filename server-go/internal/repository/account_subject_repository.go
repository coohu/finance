package repository

import (
	"context"
	"finance-server/internal/domain"
)

type accountSubjectRepository struct {
	mgr *SQLiteManager
}

func NewAccountSubjectRepository(mgr *SQLiteManager) domain.AccountSubjectRepository {
	return &accountSubjectRepository{mgr: mgr}
}

func (r *accountSubjectRepository) List(ctx context.Context, tid int64, status int) ([]*domain.AccountSubject, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}

	var list []*domain.AccountSubject
	query := db.WithContext(ctx).Order("_no")
	if status != 0 {
		query = query.Where("_isDeleted = ?", status)
	}
	err = query.Find(&list).Error
	return list, err
}

func (r *accountSubjectRepository) GetByID(ctx context.Context, tid int64, id int64) (*domain.AccountSubject, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}
	var aso domain.AccountSubject
	err = db.WithContext(ctx).First(&aso, "_id = ?", id).Error
	return &aso, err
}

func (r *accountSubjectRepository) GetByNo(ctx context.Context, tid int64, no string) (*domain.AccountSubject, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}
	var aso domain.AccountSubject
	err = db.WithContext(ctx).Where("_no = ?", no).First(&aso).Error
	return &aso, err
}

func (r *accountSubjectRepository) Save(ctx context.Context, tid int64, aso *domain.AccountSubject) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}
	return db.WithContext(ctx).Save(aso).Error
}

func (r *accountSubjectRepository) Delete(ctx context.Context, tid int64, id int64) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}
	return db.WithContext(ctx).Delete(&domain.AccountSubject{}, "_id = ?", id).Error
}
