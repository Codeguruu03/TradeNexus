import pyodbc
import sys

# ==============================
# SQL SERVER CONFIGURATION
# ==============================

SERVERS = ["10.228.50.6,1194"]  # Remote SQL Server IP
DATABASE = "CRUD"
USERNAME = "sa"
PASSWORD = "ms"

# ==============================

def try_connection(database):
    """Try to connect to SQL Server"""
    
    print(f"Connecting to {database}...")
    
    for server in SERVERS:
        try:
            # Connect to remote SQL Server
            connection_string = f"""
            DRIVER={{ODBC Driver 17 for SQL Server}};
            SERVER={server};
            DATABASE={database};
            UID={USERNAME};
            PWD={PASSWORD};
            TrustServerCertificate=yes;
            """
            
            conn = pyodbc.connect(connection_string)
            cursor = conn.cursor()
            print(f"✅ Connected to {database} on {server}")
            return conn, cursor
            
        except Exception as e:
            print(f"❌ Failed: {e}")
            continue
    
    return None, None

# =============================================
# MAIN EXECUTION
# =============================================

print("=" * 50)
print(f"SETTING UP TABLE & INITIAL DATA IN {DATABASE}")
print("=" * 50)

conn, cursor = try_connection(DATABASE)

if conn is None:
    print(f"\n❌ Could not connect to {DATABASE}")
    print("Troubleshooting:")
    print("1. Is SQL Server running?")
    print("2. Is CRUD database created?")
    print("3. Check credentials")
    sys.exit(1)

try:
    # =============================================
    # DROP EXISTING TABLE (if exists)
    # =============================================
    drop_query = """
    IF OBJECT_ID('dbo.TRADE_NEXUS_MASTER', 'U') IS NOT NULL
        DROP TABLE dbo.TRADE_NEXUS_MASTER;
    """
    
    cursor.execute(drop_query)
    cursor.commit()
    print("✅ Dropped existing table (if any)")
    
    # =============================================
    # CREATE TABLE
    # =============================================
    create_table_query = """
    CREATE TABLE dbo.TRADE_NEXUS_MASTER
    (
        TradeId INT PRIMARY KEY IDENTITY(1,1),
        
        ClientId INT,
        ClientCode VARCHAR(20),
        ClientName VARCHAR(100),
        PAN VARCHAR(20),
        City VARCHAR(50),
        State VARCHAR(50),
        MarginAvailable DECIMAL(18,2),
        
        SubBrokerId INT,
        SubBrokerCode VARCHAR(20),
        SubBrokerName VARCHAR(100),
        
        InstrumentId INT,
        Symbol VARCHAR(50),
        Segment VARCHAR(50),
        Exchange VARCHAR(50),
        LotSize INT,
        
        TradeDate DATETIME,
        BuySell VARCHAR(10),
        Quantity INT,
        Price DECIMAL(18,4),
        
        Brokerage DECIMAL(18,4),
        ExchangeCharges DECIMAL(18,4),
        SEBIFee DECIMAL(18,4),
        StampDuty DECIMAL(18,4),
        STT DECIMAL(18,4),
        GST DECIMAL(18,4),
        
        VaRPercent DECIMAL(5,2),
        ExposurePercent DECIMAL(5,2)
    );
    """
    
    cursor.execute(create_table_query)
    cursor.commit()
    print("✅ Table TRADE_NEXUS_MASTER created")
    
    # =============================================
    # INSERT SAMPLE DATA
    # =============================================
    insert_query = """
    INSERT INTO dbo.TRADE_NEXUS_MASTER 
    (ClientId, ClientCode, ClientName, PAN, City, State, MarginAvailable,
     SubBrokerId, SubBrokerCode, SubBrokerName,
     InstrumentId, Symbol, Segment, Exchange, LotSize,
     TradeDate, BuySell, Quantity, Price,
     Brokerage, ExchangeCharges, SEBIFee, StampDuty, STT, GST,
     VaRPercent, ExposurePercent)
    VALUES
    (1,'CL001','Rahul Sharma','ABCDE1234F','Mumbai','Maharashtra',500000,
     1,'SB001','Alpha Broking',
     1,'RELIANCE','CAPITAL','NSE',1,
     '2024-01-15','BUY',100,2500,
     500,50,5,25,100,90,
     12,5),
     
    (1,'CL001','Rahul Sharma','ABCDE1234F','Mumbai','Maharashtra',500000,
     1,'SB001','Alpha Broking',
     2,'TCS','CAPITAL','NSE',1,
     '2024-01-15','SELL',50,3600,
     300,30,3,10,120,60,
     10,4),
     
    (2,'CL002','Priya Mehta','FGHIJ5678K','Delhi','Delhi',750000,
     2,'SB002','Beta Capital',
     3,'NIFTY24JANFUT','FUTURES','NSE',50,
     '2024-01-16','BUY',1,22000,
     1000,100,10,50,200,180,
     15,6),
     
    (3,'CL003','Amit Verma','KLMNO9012P','Bangalore','Karnataka',300000,
     3,'SB003','Gamma Securities',
     4,'BANKNIFTY24JANFUT','FUTURES','NSE',15,
     '2024-01-16','SELL',2,48000,
     1500,150,15,75,300,270,
     18,7);
    """
    
    cursor.execute(insert_query)
    cursor.commit()
    print("✅ Sample data inserted")
    
    print("\n" + "=" * 50)
    print("🎉 Data insertion completed!")
    print("=" * 50)
    print(f"Database: {DATABASE}")
    print(f"Table: TRADE_NEXUS_MASTER")
    print(f"Records inserted: 4")
    
except Exception as e:
    print(f"\n❌ Error: {e}")
    import traceback
    traceback.print_exc()

finally:
    if cursor:
        cursor.close()
    if conn:
        conn.close()
    print("\nDatabase connection closed.")
