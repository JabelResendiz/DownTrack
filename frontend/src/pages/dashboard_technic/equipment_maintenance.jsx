import React from 'react';
import { Card,
    CardHeader, 
    CardBody,
    Typography,
    Dialog,
    DialogHeader,
    DialogBody,
    Button,
    DialogFooter,
    Input 
}from "@material-tailwind/react";

import { Pagination } from '@mui/material';
import { equipmentData } from '@/data/equipment-data';
import { useState, useEffect } from "react";
import api from '@/middlewares/api';
import DropdownMenu from '@/components/DropdownMenu';
import { useNavigate } from 'react-router-dom';
import { CheckCircleOutlineOutlined } from '@mui/icons-material';
import { PencilIcon, TrashIcon } from '@heroicons/react/24/solid';
import { useAuth } from '@/context/AuthContext';
import {convertDateFormat} from '@/utils/changeDateFormat';
import { toast } from 'react-toastify';


export function EquipmentMaintenance() {
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [currentItems, setCurrentItems] = useState([]);
    
    const [showDialog, setShowDialog] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    
    const [cost, setCost] = useState(0);
    const [selectedMaintenanceId, setSelectMaintenanceId] = useState(null);
    
    const navigate = useNavigate();
    const {user} = useAuth();


    const handleDateDisplay = (dateString) => {
        return convertDateFormat(dateString);
    };

    useEffect(()=>{
        fetchTechMaintenance(1);
    }, []);

    const options =(id) => [
        { 
            label: 'Finish maintenance', 
            className: 'text-green-500 h-5 w-5', 
            icon: CheckCircleOutlineOutlined,
            action: () => handleOpenDialog (id)
        },
        /*{ 
            label: 'Cancel maintenance', 
            className: 'text-red-500 h-5 w-5', 
            icon: TrashIcon,
            action: () => handleCancelMaintenance (id)
        },*/
    ];

    const handlePageChange = async (event, newPage) => {
        setCurrentPage(newPage);
        await fetchTechMaintenance(newPage);
    };

    const handleOpenDialog  = (id) => {
        setSelectMaintenanceId(id);
        setShowDialog(true);
    }


    const handleSubmit = async() => {
        const formattedCost = parseFloat(cost.replace(',', '.')); // Convierte la coma a punto
        try {
            const response = await api(`/DoneMaintenance/finish`, {
                method: 'PUT',
                headers: {
                    "Content-Type": "application/json",
                },
                
                body: JSON.stringify({
                    "maintenanceId": selectedMaintenanceId,
                    "cost": formattedCost,
                }),
            });

            if (response.ok) { 
                
                await fetchTechMaintenance(currentPage);
                toast.success("Maintenance completed");
                setIsLoading(false);
                setShowDialog(false);
            }
            else {
                toast.error("Maintenance incomplete")
            }
        } catch (error) {
            toast.error("Maintenance incomplete");
            setIsLoading(false);
            setShowDialog(false);
        }
    }

    const handleCancelMaintenance = async (doneMaintenanceId) => {
        try {
            const response = await api(`/DoneMaintenance/${doneMaintenanceId}`, {
                method: 'DELETE',
                headers: {
                    "Content-Type": "application/json",
                },
            });
            if (!response.ok) {
                toast.error("Failed maintenance cancellation");
                throw new Error('Network response was not ok');
            }

            await fetchTechMaintenance(currentPage);
            toast.success("Maintenance cancelled");
            setCurrentItems(currentItems.filter(item => item.id !== doneMaintenanceId));
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const fetchTechMaintenance = async (page) => {
        try {
            const response = await api(`/DoneMaintenance/Get_Maintenances_By_Technician_Status?PageNumber=${page}&PageSize=10&technicianId=${user.id}&IsFinish=false`, {
            method: 'GET',
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();

            setCurrentItems(data.items);
            setTotalPages(Math.ceil(data.totalCount / data.pageSize));

            setIsLoading(false);
        } catch (error) {
            console.error("Error fetching inventory:", error);
            setCurrentItems([]);
            setIsLoading(false);
        }
    };
    

    return (
        <>
            {(<div className={`mt-12 mb-8 flex flex-col gap-12 `}>
                <Card>
                    <CardHeader variant="gradient" color="gray" className="mb-8 p-6">
                        <Typography variant="h6" color="white">
                            Equipment Maintenance
                        </Typography>
                    </CardHeader>
                    <CardBody className="overflow-x-scroll px-0 pt-0 pb-2">
                        <table className="w-full min-w-[640px] table-auto">
                            <thead>
                                <tr>
                                    {["Equipment ID", "Equipment", "type", "date",""].map((el) => (
                                        <th
                                            key={el}
                                            className="border-b border-r border-blue-gray-50 py-3 px-5 text-left last:border-r-0 bg-gray-800"
                                        >
                                            <Typography
                                                variant="small"
                                                className="text-[11px] font-extrabold uppercase text-white"
                                            >
                                                {el}
                                            </Typography>
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody >
                                {currentItems.map(
                                    (equipment, index) => {
                                        const className = `py-3 px-5 ${index === currentItems.length - 1
                                                ? ""
                                                : "border-b border-blue-gray-50"
                                            }`;
                                        return (
                                            <tr key={equipment.equipmentId} >
                                                <td className="py-3 px-5 border-b border-r border-blue-gray-50 last:border-r-0">
                                                    <div className="flex items-center gap-4">
                                                        <div>
                                                            <Typography className="text-xs font-semibold text-blue-gray-600">
                                                                {equipment.equipmentId}
                                                            </Typography>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td className="py-3 px-5 border-b border-r border-blue-gray-50 last:border-r-0">
                                                    <div className="flex items-center gap-4">
                                                        <div>
                                                            <Typography
                                                                variant="small"
                                                                color="blue-gray"
                                                                className="font-semibold"
                                                            >
                                                                {equipment.equipmentName}
                                                            </Typography>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td className={className}>
                                                    <Typography className="text-xs font-semibold text-blue-gray-600">
                                                        {equipment.type}
                                                    </Typography>
                                                </td>

                                                <td className={className}>
                                                    <Typography className="text-xs font-semibold text-blue-gray-600">
                                                            {handleDateDisplay(equipment.date)}
                                                        </Typography>
                                                </td>
                                                <td className={className + "items-center text-center"}>
                                                    <DropdownMenu options={options(equipment.id)} />
                                                </td>
                                            </tr>
                                        );
                                    }
                                )}
                            </tbody>
                        </table>
                    </CardBody>
                    <Dialog open={showDialog} handler={() => handleShowROpen()}>
                        <DialogHeader>Insert Cost</DialogHeader>
                            <DialogBody>
                                <Input
                                    type="text"
                                    placeholder="Insert Cost"
                                    value={cost}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // Permitir solo números, coma y punto
                                        if (/^[0-9.,]*$/.test(value)) {
                                            setCost(value);
                                        }
                                    }}
                                    className="mb-4 w-full"            
                                />
                                <div className="max-h-72 overflow-y-auto mt-3">
                                </div>
                            </DialogBody>
                            <DialogFooter>
                                <Button
                                    onClick={() => handleSubmit()}
                                    variant="outlined">
                                    Accept
                                </Button>
                                <Button onClick={() => setShowDialog(false)} color="primary">
                                    Cancel
                                </Button>
                            </DialogFooter>
                    </Dialog>
                </Card>
                <Pagination
                    count={totalPages}
                    page={currentPage}
                    onChange={handlePageChange}
                    className="self-center"
                />
            </div>)
            }
        </>
    );
}

export default EquipmentMaintenance;
