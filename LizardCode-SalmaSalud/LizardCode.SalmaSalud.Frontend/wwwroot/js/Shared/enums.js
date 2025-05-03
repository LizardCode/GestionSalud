var enums = {};

enums.AjaxStatus = {
    OK: 'OK',
    Error: 'Error'
};

enums.Common = {
    Si: 'S',
    No: 'N'
};

enums.TipoUsuario = {
    Administrador: 1,
    Administracion: 2,
    Cuentas: 3,
    Tesoreria: 4,
    CuentasPorPagar: 5,
    CuentasPorCobrar: 6,
    Profesional: 7,
    Recepcion: 8,
    Paciente: 9,
    ProfesionalExterno: 10
}

enums.MaskConstraintType = {
    RegExp: "regexp",
    String: "string",
    Number: "number",
    Date: "date",
    Custom: "custom"
};

enums.FormatoFecha = {
    DatabaseFormat: "YYYY-MM-DD",
    DatabaseFullFormat: "YYYY-MM-DD HH:mm:ss",
    DefaultFormat: "DD/MM/YYYY",
    DefaultFullFormat: "DD/MM/YYYY HH:mm:ss"
};

enums.AutoNumericConstraintType = {
    Numeric: "numeric",
    Currency: "currency",
    Percentage: "percentage"
};

enums.TiposIVA = {
    ConsumidorFinal: 1,
    ResponsableInscripto: 2,
    Monotributo: 3,
    Exento: 4,
    ExentoNoGravado: 5,
    Exterior: 6
};

enums.TiposRetencion = {
    Ganancias: 1,
    IVA: 2,
    IngresosBrutos: 3,
    SUSS: 4,
    IVAMonotributo: 5,
    GananciasMonotributo: 6
};

enums.CategoriaRetencion = {
    Ganancias: 1,
    IVA: 2,
    IngresosBrutos: 3,
    SUSS: 4
};

enums.EstadoCheque = {
    SinLibrar: 1,
    Librado: 2,
    DebitadoDepositado: 3,
    Anulado: 4,
    Rechazado: 5,
    DebitadoRechazado: 6,
    EnCartera: 7,
    Entregado: 8
};

enums.EstadoAFIP = {
    Inicial: 1,
    Observado: 2,
    Error: 3,
    Autorizado: 4,
    DocumentoSinCAE: 5
};

enums.TipoCheque = {
    Comun: 1,
    Diferido: 2,
    EChequeComun: 3,
    EChequeDiferido: 4,
    Terceros: 5
};

enums.TipoItemEstimado = {
    Costos: 1,
    Gastos: 2
};

enums.EstadoEstimado = {
    Emitido: 1,
    AprobadoCliente: 2,
    AprobadoFacturar: 3,
    FacturadoParcial: 4,
    FacturadoTotal: 5
};

enums.Monedas = {
    MonedaLocal: 'PES'
};

enums.EstadoRecibo = {
    Ingresado: 1,
    Finalizado: 2,
    Anulado: 3
};

enums.EstadoOrdenPago = {
    Ingresada: 1,
    Pagada: 2
};

enums.TipoCobro = {
    Efectivo: 1,
    Cheque: 2,
    Transferencia: 3,
    Documento: 4
};

enums.TipoPago = {
    Efectivo: 1,
    ChequeComun: 2,
    ChequeDiferido: 3,
    EChequeComun: 4,
    EChequeDiferido: 5,
    ChequeTerceros: 6,
    Transferencia: 7,
    CuentaContable: 8
};

enums.DescripcionTipoCheque = {
    ChequeComun: "ChequeComun",
    ChequeDiferido: "ChequeDiferido",
    EChequeComun: "EChequeComun",
    EChequeDiferido: "EChequeDiferido",
    ChequeTerceros: "ChequeTerceros",
};

enums.TipoSdoInicioBanco = {
    ChequeComun: 1,
    ChequeDiferido: 2,
    EChequeComun: 3,
    EChequeDiferido: 4,
    ChequeTerceros: 5
};

enums.TipoDeposito = {
    Efectivo: 1,
    ChequeComun: 2,
    ChequeDiferido: 3,
    EChequeComun: 4,
    EChequeDiferido: 5,
    ChequeTerceros: 6,
    Transferencia: 7
};

enums.Comprobante = {
    FACTURA_A: 1,
    FACTURA_B: 2,
    FACTURA_C: 3,
    FACTURA_E: 4,
    NCREDITO_A: 5,
    NCREDITO_B: 6,
    NCREDITO_C: 7,
    NCREDITO_E: 8,
    NDEBITO_A: 9,
    NDEBITO_B: 10,
    NDEBITO_C: 11,
    NDEBITO_E: 12,
    TICKET_A: 13,
    TICKET_B: 14,
    TICKET_C: 15,
    FACTURA_MIPYME_A: 16,
    NDEBITO_MIPYME_A: 17,
    NCREDITO_MIPYME_A: 18,
    FACTURA_MIPYME_B: 19,
    NDEBITO_MIPYME_B: 20,
    NCREDITO_MIPYME_B: 21,
    FACTURA_MIPYME_C: 22,
    NDEBITO_MIPYME_C: 23,
    NCREDITO_MIPYME_C: 24,
    FACTURAS_M: 25,
    NDEBITO_M: 26,
    NCREDITO_M: 27,
    OTROS_COMPROBANTES: 28,
};

enums.TipoComprobante = {
    AutomaticaClientes: 1,
    ManualClientes: 2,
    ManualArticulos: 3,
    AutomaticaProveedores: 4,
    ManualProveedores: 5,
    AutomaticaAnulaFacturaClientes: 6,
    AutomaticaAnulaFacturaProveedores: 7,
    GastosProveedores: 8,
    GastosBancarios: 9,
    InterfazAFIP: 10
};

enums.TipoDocumento = {
    CUIT: 80,
    CUIL: 86,
    CDI: 87,
    LibretaEnrolamiento: 89,
    LibretaCivica: 90,
    Pasaporte: 94,
    DNI: 96
};

enums.EstadoPlanillaGastos = {
    Ingresada: 1,
    Pagada: 2
};

enums.Modulos = {
    Clientes: "CLI",
    Proveedores: "PRO",
    CajaBanco: "CAJ"
};

enums.TipoOrdenPago = {
    Proveedores: 1,
    Gastos: 2,
    Anticipo: 3,
    Varios: 4
};

enums.TipoRecibo = {
    Comun: 1,
    Anticipo: 2
};

enums.TipoInterfaz = {
    MisComprobantes: 0,
    LinksideFacturante: 1,
    DawaSoft: 2
};

enums.EstadoTurno = {
    Agendado: 1,
    Confirmado: 2,
    AusenteConAviso: 3,
    AusenteSinAviso: 4,
    Recepcionado: 5,
    Atendido: 6,
    Cancelado: 7,
    ReAgendado: 8
};

enums.TipoTurno = {
    Turno: 0,
    SobreTurno: 1,
    DemandaEspontanea: 2,
    Guardia: 3
};

enums.EstadoPresupuesto = {
    Abierto: 1,
    Aprobado: 2,
    Rechazado: 3,
    Vencido: 4,
    Cerrado: 5
};

enums.EstadoPedidoLaboratorio = {
    Pendiente: 1,
    Envviado: 2,
    Recibido: 3
};

enums.EstadoTurnoSolicitud = {
    Solicitado: 1,
    Asignado: 2,
    Cancelado: 3
};