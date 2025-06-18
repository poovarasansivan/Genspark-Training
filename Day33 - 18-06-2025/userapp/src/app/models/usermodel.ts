export class UserModel {
  constructor(
    public id: number,
    public image: string,
    public firstName: string,
    public lastName: string,
    public gender: string,
    public age: number,
    public email: string,
    public phone: string,
    public username: string,
    public password: string,
    public address: {
      state: string;
    },
    public role: string
  ) {}
}
