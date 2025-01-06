import React, { useState, useContext } from 'react';
import styles from './TimeLog.module.css'; // Importação de estilos para organização

const TimeLog: React.FC = () => {
    return (
        <div className={styles.container}>
            <div className={styles.formWrapper}>
                <h1>TimeLog</h1>
            </div>
        </div>
    );
};

export default TimeLog;