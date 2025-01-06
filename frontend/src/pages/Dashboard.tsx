import React, { useState, useContext } from 'react';
import styles from './Dashboard.module.css'; // Importação de estilos para organização

const Dashboard: React.FC = () => {
    return (
        <div className={styles.container}>
            <div className={styles.formWrapper}>
                <h1>Dashboard</h1>
            </div>
        </div>
    );
};

export default Dashboard;