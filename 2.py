import pyodbc
import random
import sys

# ==============================
# SQL SERVER CONFIGURATION
# ==============================

SERVER = "10.228.50.6,1194"  # Remote SQL Server IP
DATABASE = "CRUD"
USERNAME = "sa"
PASSWORD = "ms"

# ==============================

def connect_to_db():
    """Connect to SQL Server"""
    
    try:
        connection_string = f"""
        DRIVER={{ODBC Driver 17 for SQL Server}};
        SERVER={SERVER};
        DATABASE={DATABASE};
        UID={USERNAME};
        PWD={PASSWORD};
        TrustServerCertificate=yes;
        """
        
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()
        print(f"✅ Connected to {DATABASE}")
        return conn, cursor
        
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return None, None

# =============================================
# GENERATE AND INSERT MORE DATA
# =============================================

print("=" * 60)
print("INSERTING MORE TRADE DATA INTO TRADE_NEXUS_MASTER")
print("=" * 60)

conn, cursor = connect_to_db()

if conn is None:
    sys.exit(1)

try:
    # Client data - more clients
    clients = [
        (7,'CL007','Ritika Sharma','POIUY3456Q','Jaipur','Rajasthan',300000,7,'SB007','Theta Capital'),
        (8,'CL008','Vikram Singh','LKJHG7890R','Lucknow','Uttar Pradesh',700000,8,'SB008','Iota Investments'),
        (9,'CL009','Anjali Nair','MNBVC2345T','Kochi','Kerala',550000,9,'SB009','Kappa Traders'),
        (10,'CL010','Rohit Malhotra','YUIOP6789S','Pune','Maharashtra',800000,10,'SB010','Lambda Broking'),
        (11,'CL011','Meera Iyer','GHJKL1122Z','Chennai','Tamil Nadu',620000,11,'SB011','Mu Securities'),
        (12,'CL012','Deepak Yadav','TREWA3344X','Indore','Madhya Pradesh',400000,12,'SB012','Nu Investments'),
        (13,'CL013','Sneha Joshi','PLMOK5566V','Nagpur','Maharashtra',720000,13,'SB013','Xi Capital'),
        (14,'CL014','Kunal Desai','RFVTG7788B','Surat','Gujarat',350000,14,'SB014','Omicron Broking'),
        (15,'CL015','Ishita Roy','EDCFR9900D','Kolkata','West Bengal',950000,15,'SB015','Pi Securities'),
        (16,'CL016','Harsh Vardhan','ZAQWS1123F','Bhopal','Madhya Pradesh',520000,16,'SB016','Rho Investments'),
        (17,'CL017','Tanya Arora','XSWED4456H','Noida','Uttar Pradesh',680000,17,'SB017','Sigma Broking'),
        (18,'CL018','Manish Gupta','CVFRT7788J','Kanpur','Uttar Pradesh',480000,18,'SB018','Tau Capital'),
        (19,'CL019','Divya Menon','BGYHN9911K','Thiruvananthapuram','Kerala',610000,19,'SB019','Upsilon Traders'),
        (20,'CL020','Siddharth Jain','NHYUJ2233L','Vadodara','Gujarat',770000,20,'SB020','Phi Securities'),
        (21,'CL021','Pooja Chawla','KJHGF5566P','Mysore','Karnataka',660000,21,'SB021','Chi Investments'),
        (22,'CL022','Nitin Agarwal','QAZXC7788Q','Patna','Bihar',430000,22,'SB022','Psi Capital'),
        (23,'CL023','Kavita Rao','WSXED9900R','Visakhapatnam','Andhra Pradesh',880000,23,'SB023','Omega Broking'),
        (24,'CL024','Sanjay Patel','QWERT1234L','Ahmedabad','Gujarat',600000,4,'SB024','Delta Investments'),
        (25,'CL025','Neha Kapoor','ZXCVB5678M','Chandigarh','Punjab',450000,5,'SB025','Epsilon Broking'),
        (26,'CL026','Arjun Reddy','ASDFG9012N','Hyderabad','Telangana',900000,6,'SB026','Zeta Securities'),
    ]
    
    # Instruments
    instruments = [
        (11,'BAJFINANCE','CAPITAL','NSE',1,16,6),
        (12,'HINDUNILVR','CAPITAL','NSE',1,9,3),
        (13,'ASIANPAINT','CAPITAL','NSE',1,11,4),
        (14,'WIPRO','CAPITAL','NSE',1,12,5),
        (15,'TECHM','CAPITAL','NSE',1,13,5),
        (16,'ULTRACEMCO','CAPITAL','NSE',1,14,6),
        (17,'POWERGRID','CAPITAL','NSE',1,8,3),
        (18,'TITAN','CAPITAL','NSE',1,15,6),
        (19,'ADANIENT','CAPITAL','NSE',1,20,8),
        (20,'JSWSTEEL','CAPITAL','NSE',1,17,7),
        (21,'COALINDIA','CAPITAL','NSE',1,9,3),
        (22,'NTPC','CAPITAL','NSE',1,8,3),
        (23,'ONGC','CAPITAL','NSE',1,10,4),
        (24,'SUNPHARMA','CAPITAL','NSE',1,11,4),
        (25,'HCLTECH','CAPITAL','NSE',1,13,5)
    ]
    
    # Generate 496 random trade records (to make total 500 with 4 initial records)
    records = []
    for i in range(496):
        c = random.choice(clients)
        inst = random.choice(instruments)
        
        qty = random.randint(10, 500)
        price = random.randint(400, 5000)
        buy_sell = random.choice(['BUY','SELL'])
        day = random.randint(1,28)
        lot_size = random.randint(1, 100)  # Random lot size
        
        records.append((
            c[0], c[1], c[2], c[3], c[4], c[5], c[6],  # Client
            c[7], c[8], c[9],  # SubBroker
            inst[0], inst[1], inst[2], inst[3], lot_size,  # Instrument with random lot size
            f'2026-02-{day:02d}', buy_sell, qty, price,  # Trade
            random.randint(100,1500), random.randint(10,150),  # Brokerage, ExchangeCharges
            random.randint(1,15), random.randint(5,80),  # SEBIFee, StampDuty
            random.randint(50,300), random.randint(40,250),  # STT, GST
            inst[5], inst[6]  # VaR, Exposure
        ))
    
    # Insert all records
    insert_query = """
    INSERT INTO dbo.TRADE_NEXUS_MASTER 
    (ClientId, ClientCode, ClientName, PAN, City, State, MarginAvailable,
     SubBrokerId, SubBrokerCode, SubBrokerName,
     InstrumentId, Symbol, Segment, Exchange, LotSize,
     TradeDate, BuySell, Quantity, Price,
     Brokerage, ExchangeCharges, SEBIFee, StampDuty, STT, GST,
     VaRPercent, ExposurePercent)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """
    
    cursor.executemany(insert_query, records)
    cursor.commit()
    print(f"✅ {len(records)} new records inserted")
    
    # Show total records
    count_query = "SELECT COUNT(*) as Total FROM dbo.TRADE_NEXUS_MASTER"
    cursor.execute(count_query)
    result = cursor.fetchone()
    print(f"✅ Total records in table: {result[0]}")
    
    print("\n" + "=" * 60)
    print("🎉 Data insertion completed successfully!")
    print("=" * 60)
    
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