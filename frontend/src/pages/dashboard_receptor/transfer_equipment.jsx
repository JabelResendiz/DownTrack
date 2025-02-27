import React from 'react';
import { Card, CardHeader, CardBody, Typography, Button, Dialog, DialogHeader,DialogFooter, DialogBody, Input,
} from "@material-tailwind/react";
import { InformationCircleIcon, CheckCircleIcon  } from "@heroicons/react/24/outline";
import { useState, useEffect } from "react";
import TransferInfoForm from "./info_transfer";
import { Pagination } from '@mui/material';
import { toast } from 'react-toastify';
import api from "@/middlewares/api";
import { useAuth } from '@/context/AuthContext';
import DropdownMenu from '@/components/DropdownMenu';
import { convertDateFormat } from '@/utils';

export function EquipmentTransferTable() {
    const [onInfo, setOnInfo] = useState(false);
    const [selectedTransfer, setSelectedTransfer] = useState(null);
    const [isRegistered, setIsRegistered] = useState(false);
    const [registeredTransfers, setRegisteredTransfers] = useState([]); 
    

    const [showRegistrationForm, setShowRegistrationForm] = useState(false);
    const [shippingSupervisors, setShippingSupervisors] = useState([]);
    const [selectedSupervisor, setSelectedSupervisor] = useState(null);

    const [dateFormat, setDateFormat] = useState("");
    const [startDate, setStartDate] = useState("");

    const [searchQuery, setSearchQuery] = useState("");
    const [filteredShippingS, setFilteredShippingS] = useState([]);

    const [isLoading, setIsLoading] = useState(true);

    const [totalPages, setTotalPages] = useState(0);
    const [currentItems, setCurrentItems] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);

    const { user } = useAuth();

    const options =(transfer) => [
        { 
            label: 'Information',
            className: 'text-blue-500 h-5 w-5', 
            icon: InformationCircleIcon,
            action: () => handleShowInfo(transfer)
        },
        { 
            label: 'Register',
            className: 'text-green-500 h-5 w-5', 
            icon: CheckCircleIcon,
            action: () => handleRegister(transfer)
        },
    ];

    useEffect(() => {
        setIsLoading(true);
        const currentDate = generateDateTime();
        setStartDate(currentDate);
        const dateInFormat = inFormatDate();
        setDateFormat(dateInFormat);
        fetchTransfers(1);
        fetchShippingSupervisors();
        setIsLoading(false);
    }, []);
    

    const handleDateDisplay = (dateString) => {
        return convertDateFormat(dateString);
    };
    const handleShowInfo = (transfer) => {
        console.log("selected tranfer", transfer);
        setSelectedTransfer(transfer);
        setOnInfo(true);
    };

    const handleCloseInfo = () => {
        setSelectedTransfer(null);
        setOnInfo(false);
    };

    const handleRegister = (transfer) => {
        setShowRegistrationForm(true);
        setSelectedTransfer(transfer);
    };


    const handlePageChange = async (event, newPage) => {
        setCurrentPage(newPage);
        await fetchTransfers(newPage);
    };

    const handleOpenDialog  = (id) => {
        setSelectedTransfer(id);
        setShowRegistrationForm(true);
    }

    const handleShippingSelect = (shippingId, shippingName) => {
        setSelectedSupervisor({
            id: shippingId,
            name: shippingName
        });
    };

    const generateDateTime = () => {
        const currentDate = new Date();
        return currentDate
            .toISOString()
            .slice(0, 19) // Recorta para obtener la fecha y hora en formato YYYY-MM-DDTHH:MM:SS
            .replace("T", " "); // Reemplaza "T" con espacio para formato MySQL
    };

    const inFormatDate = () => {
        const currentDate = new Date();
        const year = currentDate.getFullYear();
        const month = String(currentDate.getMonth() + 1).padStart(2, '0');
        const day = String(currentDate.getDate()).padStart(2, '0');
        const hours = String(currentDate.getHours()).padStart(2, '0');
        const minutes = String(currentDate.getMinutes()).padStart(2, '0');
        const seconds = String(currentDate.getSeconds()).padStart(2, '0');
        const milliseconds = String(currentDate.getMilliseconds()).padStart(3, '0');
    
        return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${milliseconds}Z`;
    };

    const fetchTransfers = async (page) => {
        try {
            const response = await api(`/TransferRequest/GetByArrivalDepartment/${user.id}?PageNumber=${page}&PageSize=10`, {
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
            console.error("Error fetching employees:", error);
            setCurrentItems([]);
            setIsLoading(false);
        }
    };

    const fetchShippingSupervisors = async () => {
        try {
            const response = await api('/Employee/GetAllShippingSupervisor', {
                method: 'GET',
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            console.log("Responsibles data", data);
            setShippingSupervisors(data);
        } catch (error) {
            console.error("Error fetching responsibles:", error);
        }
    };

    const handleSubmit = async () => {
        try {
            const response = await api('/Transfer/POST', {
                method: 'POST',
                body: JSON.stringify({
                    "requestId": selectedTransfer.id,
                    "shippingSupervisorId": selectedSupervisor.id,
                    "equipmentReceptorId": parseInt(user.id),
                    "date": dateFormat,
                }),
            });

            const data = await response.json();
            if (!response.ok) {
                toast.error('Error saving transfer');
                throw new Error('Network response was not ok');
            }
            
            toast.success('Transfer saved successfully')
            setShowRegistrationForm(false);
            await fetchTransfers(1);

            console.log("Transfer saved successfully:", data);
            // Handle success (e.g., show a success message, update UI, etc.)
        } catch (error) {
            console.error("Error saving transfer:", error);
            // Handle error (e.g., show an error message, etc.)
        }
    };

    const handleSearch = (e) => {
        const query = e.target.value;
        setSearchQuery(query);
        
        if (query) {
            setFilteredShippingS(
                shippingSupervisors.filter(
                    (receptor) => receptor.name.toLowerCase().includes(query.toLowerCase())
                )
            );
        } else {
            setFilteredShippingS(shippingSupervisors);
        }
    };


return (
    <>
        {onInfo && selectedTransfer && (
            <TransferInfoForm 
                transfer={selectedTransfer}
                onClose={handleCloseInfo}
            />
        )}
        
        { !onInfo &&
            (<div className={`mt-12 mb-8 flex flex-col gap-12 ${showRegistrationForm ? 'blur-background' : ''}`}>
            <Card>
                <CardHeader variant="gradient" color="gray" className="mb-8 p-6">
                    <Typography variant="h6" color="white">
                        Equipment Transfer
                    </Typography>
                </CardHeader>
                <CardBody className="overflow-x-scroll px-0 pt-0 pb-2">
                    <table className="w-full min-w-[640px] table-auto">
                        <thead>
                            <tr>
                                {[ "Source Section","Source Department", "Equipment","Type","Date", ""].map((el) => (
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
                        <tbody>
                            {currentItems.map(
                                (transfer, index) => {
                                    const className = `py-3 px-5 ${
                                        index === currentItems.length - 1
                                            ? ""
                                            : "border-b border-blue-gray-50"
                                    }`;
                                    return (
                                        <tr key={transfer.id}>
                                            <td className={className}>
                                                <div className="flex items-center gap-4">
                                                    <div>
                                                        <Typography className="text-xs font-semibold text-blue-gray-600">
                                                            {transfer.sourceSectionName}
                                                        </Typography>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className={className}>
                                                <Typography className="text-xs font-semibold text-blue-gray-600">
                                                    {transfer.sourceDepartmentName}
                                                </Typography>
                                            </td>
                                            <td className={className}>
                                                <Typography className="text-xs font-semibold text-blue-gray-600">
                                                    {transfer.equipmentName}
                                                </Typography>
                                            </td>
                                            <td className={className}>
                                                <Typography className="text-xs font-semibold text-blue-gray-600">
                                                    {transfer.equipmentType}
                                                </Typography>
                                            </td>
                                            <td className={className}>
                                                <Typography className="text-xs font-semibold text-blue-gray-600">
                                                    {handleDateDisplay(transfer.date)}
                                                </Typography>
                                            </td>
                                            <td className={className}>
                                                <div className="flex items-center gap-4">
                                                    <td className={className + "items-center text-center"}>
                                                            <DropdownMenu options={options(transfer)} />
                                                </td>
                                                </div>
                                            </td>
                                        </tr>
                                    );
                                }
                            )}
                        </tbody>
                    </table>
                </CardBody>
                    <Dialog open={showRegistrationForm} handler={() => handleOpenDialog()}>
                    <DialogHeader>Select Shipping Supervisor</DialogHeader>
                        <DialogBody>
                        <Input
                            type="text"
                            placeholder="Search Shipping Supervisor"
                            value={searchQuery}
                            onChange={(e) => handleSearch(e, "shippingS")}
                            className="mb-4 w-full"            
                        />
                        <div className="max-h-72 overflow-y-auto mt-3">
                            {filteredShippingS.map((shipping) => (
                            <div
                                key={shipping.id}
                                className={`p-2 cursor-pointer hover:bg-gray-300 ${selectedSupervisor?.id === shipping.id ? 'bg-gray-200' : ''}`}
                                onClick={() => handleShippingSelect(shipping.id, shipping.name)}
                            >
                                {shipping.name}
                            </div>
                            ))}
                        </div>
                        </DialogBody>
                    <DialogFooter>
                            <Button onClick={() => setShowRegistrationForm(false)} color="primary">
                                Cancel
                            </Button>
                            <Button className='ml-2' onClick={() => {handleSubmit()}} color="primary">
                                Accept
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

export default EquipmentTransferTable;
