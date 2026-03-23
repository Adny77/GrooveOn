import 'package:json_annotation/json_annotation.dart';

part 'user.g.dart';

@JsonSerializable()
class User {
  int id;
  String? firstName;
  String? lastName;
  String? username;
  String? email;
  String? userImage;
  DateTime? dateOfBirth;
  String? phoneNumber;
  bool isActive;
  DateTime? joinDate;
  DateTime? lastLogin;

  User({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.username,
    required this.email,
    this.userImage,
    this.dateOfBirth,
    this.phoneNumber,
    required this.isActive,
    required this.joinDate,
    this.lastLogin,
  });

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);

  Map<String, dynamic> toJson() => _$UserToJson(this);
}