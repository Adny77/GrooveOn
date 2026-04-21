import 'package:json_annotation/json_annotation.dart';

part 'user.g.dart';

@JsonSerializable()
class User {
  final int id;
  final String firstName;
  final String lastName;
  final String username;

  final String? passwordHash;
  final String email;
  final String? userImage;
  final DateTime? dateOfBirth;
  final String? phoneNumber;
  final bool isActive;
  final DateTime joinDate;
  final DateTime? lastLogin;

  final List<String>? roles;

  User({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.username,
    this.passwordHash,
    required this.email,
    this.userImage,
    this.dateOfBirth,
    this.phoneNumber,
    required this.isActive,
    required this.joinDate,
    this.lastLogin,
    this.roles,
  });

  factory User.fromJson(Map<String, dynamic> json) =>
      _$UserFromJson(json);

  Map<String, dynamic> toJson() => _$UserToJson(this);
}