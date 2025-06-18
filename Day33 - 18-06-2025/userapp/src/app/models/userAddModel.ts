export class UserAddModel {
  constructor(
    public firstName: string,
    public lastName: string,
    public maidenName: string,
    public age: number,
    public gender: string,
    public email: string,
    public phone: string,
    public username: string,
    public password: string,
    public image: string,
    public bloodGroup: string,
    public height: number,
    public weight: number,
    public eyecolor: string,
    public hair: {
      color: string;
      type: string;
    },
    public ip: string,
    public address: {
      address: string;
      city: string;
      state: string;
      postalCode: string;
      coordinates: {
        lat: number;
        lng: number;
      };
      country: string;
    },
    public macAddress: string,
    public university: string,
    public bank: {
      cardExpire: string,
      cardNumber: string,
      cardType: string,
      currency: string,
      iban: string,
    },
    public company: {
      department: string,
      name: string,
      title: string,
      address: {
        address: string,
        city: string,
        state: string,
        postalCode: string,
        coordinates: {
          lat: number,
          lng: number
        };
      };
    country: string,
    },
    public ein: string,
    public ssn: string,
    public userAgent: string,
    public crypto: {
        coin : string,
        wallet: string,
        network: string
    },
    public role: string,
  ) {}
}
