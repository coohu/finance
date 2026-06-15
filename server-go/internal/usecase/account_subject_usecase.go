package usecase

import (
	"context"
	"finance-server/internal/domain"
)

type AccountSubjectUsecase interface {
	List(ctx context.Context, tid int64, status int) ([]*domain.AccountSubject, error)
	Save(ctx context.Context, tid int64, aso *domain.AccountSubject) error
	Delete(ctx context.Context, tid int64, id int64) error
}

type accountSubjectUsecase struct {
	repo domain.AccountSubjectRepository
}

func NewAccountSubjectUsecase(repo domain.AccountSubjectRepository) AccountSubjectUsecase {
	return &accountSubjectUsecase{repo: repo}
}

func (u *accountSubjectUsecase) List(ctx context.Context, tid int64, status int) ([]*domain.AccountSubject, error) {
	return u.repo.List(ctx, tid, status)
}

func (u *accountSubjectUsecase) Save(ctx context.Context, tid int64, aso *domain.AccountSubject) error {
	return u.repo.Save(ctx, tid, aso)
}

func (u *accountSubjectUsecase) Delete(ctx context.Context, tid int64, id int64) error {
	return u.repo.Delete(ctx, tid, id)
}
