import React from 'react';
import { Typography, Button, Container, Box } from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { useNavigate } from 'react-router-dom'; // Para redireccionar al usuario

const Error403 = () => {
    const navigate = useNavigate();

    const handleGoHome = () => {
    navigate('/'); // Redirige al usuario a la página de inicio
    };

    return (
    <Container
        sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '100vh',
        textAlign: 'center',
        backgroundColor: '#f5f5f5',
        }}
    >
        <Box
        sx={{
            backgroundColor: 'white',
            padding: '40px',
            borderRadius: '10px',
            boxShadow: '0px 4px 20px rgba(0, 0, 0, 0.1)',
        }}
        >
        <LockOutlinedIcon
            sx={{
            fontSize: '80px',
            color: '#ff4444',
            marginBottom: '20px',
            }}
        />
        <Typography variant="h4" sx={{ fontWeight: 'bold', color: '#333', marginBottom: '10px' }}>
            Error 403: Acceso Prohibido
        </Typography>
        <Typography variant="body1" sx={{ color: '#666', marginBottom: '30px' }}>
            No tienes permiso para acceder a esta página.
        </Typography>
        <Button
            variant="contained"
            onClick={handleGoHome}
            sx={{
            backgroundColor: '#ff4444',
            color: 'white',
            fontWeight: 'bold',
            padding: '10px 30px',
            borderRadius: '5px',
            '&:hover': {
                backgroundColor: '#cc0000',
            },
            }}
        >
            Volver al Inicio
        </Button>
        </Box>
    </Container>
    );
};

export default Error403;