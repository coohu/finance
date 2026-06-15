package repository

import (
	"fmt"
	"os"
	"sync"

	"gorm.io/driver/sqlite"
	"gorm.io/gorm"
)

type SQLiteManager struct {
	dbs    map[int64]*gorm.DB
	mu     sync.RWMutex
	dbPath string
}

func NewSQLiteManager(dbPath string) *SQLiteManager {
	if _, err := os.Stat(dbPath); os.IsNotExist(err) {
		os.MkdirAll(dbPath, 0755)
	}
	return &SQLiteManager{
		dbs:    make(map[int64]*gorm.DB),
		dbPath: dbPath,
	}
}

func (m *SQLiteManager) GetDB(tid int64) (*gorm.DB, error) {
	m.mu.RLock()
	db, ok := m.dbs[tid]
	m.mu.RUnlock()
	if ok {
		return db, nil
	}

	m.mu.Lock()
	defer m.mu.Unlock()

	if db, ok := m.dbs[tid]; ok {
		return db, nil
	}

	var dsn string
	if tid == -1 {
		dsn = fmt.Sprintf("%s/main.db", m.dbPath)
	} else {
		dsn = fmt.Sprintf("%s/account_%d.db", m.dbPath, tid)
	}

	db, err := gorm.Open(sqlite.Open(dsn), &gorm.Config{})
	if err != nil {
		return nil, err
	}

	m.dbs[tid] = db
	return db, nil
}
